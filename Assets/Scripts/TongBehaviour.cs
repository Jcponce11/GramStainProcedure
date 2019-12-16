namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;

    public class TongBehaviour : VRTK_InteractGrab
    {

        [Header("Use Settings")]

        [Tooltip("The button used to grab/release a touched Interactable Object.")]
        public VRTK_ControllerEvents.ButtonAlias useButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
        [Tooltip("An amount of time between when the grab button is pressed to when the controller is touching an Interactable Object to grab it.")]
        public float usePrecognition = 0f;
      /*  [Tooltip("An amount to multiply the velocity of any Interactable Object being thrown.")]
        public float throwMultiplier = 1f;
        [Tooltip("If this is checked and the Interact Touch is not touching an Interactable Object when the grab button is pressed then a Rigidbody is added to the interacting object to allow it to push other Rigidbody objects around.")]
        public bool createRigidBodyWhenNotTouching = false; */

        [Header("Custom Settings")]

        [Tooltip("The rigidbody point on the controller model to snap the grabbed Interactable Object to. If blank it will be set to the SDK default.")]
        public Rigidbody controllerTongAttachPoint = null;
        [Tooltip("The Controller Events to listen for the events on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_ControllerEvents controllerTongEvents;
        [Tooltip("The Interact Touch to listen for touches on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_InteractTouch interactTongTouch;
        public Material touchDebug;
        private Material untouchedMaterial;





        /// <summary>
        /// The StartGrab method sets up the grab attach mechanic as soon as an Interactable Object is grabbed. It is also responsible for creating the joint on the grabbed object.
        /// </summary>
        /// <param name="usingObject">The GameObject that is doing the using.</param>
        /// <param name="givenUsedObject">The GameObject that is being used.</param>
        /// <param name="givenControllerAttachPoint">The point on the grabbing object that the grabbed object should be attached to after grab occurs.</param>
        /// <returns>Returns `true` if the grab is successful, `false` if the grab is unsuccessful.</returns>

        public event ControllerInteractionEventHandler UseButtonPressed;
        public event ControllerInteractionEventHandler UseButtonReleased;
        public event ObjectInteractEventHandler ControllerStartUseInteractableObject;
        public event ObjectInteractEventHandler ControllerUseInteractableObject;
        public event ObjectInteractEventHandler ControllerStartUnUseInteractableObject;
        public event ObjectInteractEventHandler ControllerUnUseInteractableObject;

        protected VRTK_ControllerEvents.ButtonAlias subscribedUseButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        protected VRTK_ControllerEvents.ButtonAlias savedUseButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        protected bool usePressed;

        protected GameObject usedObject = null;
        protected bool influencingUsedObject = false;
        protected int useEnabledState = 0;
        protected float usePrecognitionTimer = 0f;
        protected GameObject undroppableUsedObject;
        protected Rigidbody originalControllerTongAttachPoint;

        protected VRTK_ControllerReference controllerTongReference
        {
            get
            {
                return VRTK_ControllerReference.GetControllerReference((interactTongTouch != null ? interactTongTouch.gameObject : null));
            }
        }

        public virtual void OnControllerStartUseInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerStartUseInteractableObject != null)
            {
                ControllerStartUseInteractableObject(this, e);
            }
        }

        public virtual void OnControllerUseInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerUseInteractableObject != null)
            {
                ControllerUseInteractableObject(this, e);
            }
        }

        public virtual void OnControllerStartUnUseInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerStartUnUseInteractableObject != null)
            {
                ControllerStartUnUseInteractableObject(this, e);
            }
        }

        public virtual void OnControllerUnUseInteractableObject(ObjectInteractEventArgs e)
        {
            if (ControllerUnUseInteractableObject != null)
            {
                ControllerUnUseInteractableObject(this, e);
            }
        }

        public virtual void OnUseButtonPressed(ControllerInteractionEventArgs e)
        {
            if (UseButtonPressed != null)
            {
                UseButtonPressed(this, e);
            }
        }

        public virtual void OnUseButtonReleased(ControllerInteractionEventArgs e)
        {
            if (UseButtonReleased != null)
            {
                UseButtonReleased(this, e);
            }
        }

        public virtual bool IsUseButtonPressed()
        {
            return usePressed;
        }

        public virtual void ForceUseRelease(bool applyGrabbingObjectVelocity = false)
        {
            InitUnUsedObject(applyGrabbingObjectVelocity);
        }

        public virtual void AttemptUse()
        {
            AttemptUseObject();
        }

        public virtual GameObject GetUsedObject()
        {
            return usedObject;
        }

        public virtual void ForceControllerTongAttachPoint(Rigidbody forcedAttachPoint)
        {
            originalControllerTongAttachPoint = forcedAttachPoint;
            controllerTongAttachPoint = forcedAttachPoint;
        }

        protected override void Awake()
        {
            originalControllerTongAttachPoint = controllerTongAttachPoint;
            controllerEvents = (controllerEvents != null ? controllerEvents : GetComponentInParent<VRTK_ControllerEvents>());
            interactTongTouch = (interactTongTouch != null ? interactTongTouch : GetComponentInParent<VRTK_InteractTouch>());
            if (interactTongTouch == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_InteractGrab", "VRTK_InteractTouch", "interactTouch", "the same or parent"));
            }

            VRTK_SDKManager.AttemptAddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected override void OnEnable()
        {
            RegrabUndroppableObject();
            ManageGrabListener(true);
            ManageInteractTouchListener(true);
            if (controllerEvents != null)
            {
                controllerEvents.ControllerIndexChanged += DoControllerModelUpdate;
                controllerEvents.ControllerModelAvailable += DoControllerModelUpdate;
            }
            SetControllerAttachPoint();
        }

        protected override void OnDisable()
        {
            SetUndroppableObject();
            ForceRelease();
            ManageGrabListener(false);
            ManageInteractTouchListener(false);
            if (controllerEvents != null)
            {
                controllerEvents.ControllerIndexChanged -= DoControllerModelUpdate;
                controllerEvents.ControllerModelAvailable -= DoControllerModelUpdate;
            }
        }

        protected override void OnDestroy()
        {
            VRTK_SDKManager.AttemptRemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected override void Update()
        {
            ManageGrabListener(true);
            CheckControllerAttachPointSet();
            CreateNonTouchingRigidbody();
            CheckPrecognitionGrab();
        }
        
        protected override void DoControllerModelUpdate(object sender, ControllerInteractionEventArgs e)
        {
            SetControllerAttachPoint();
        }

        protected override void ManageInteractTouchListener(bool state)
        {
            if (interactTongTouch != null && !state)
            {
                interactTongTouch.ControllerTouchInteractableObject -= ControllerTouchInteractableObject;
                interactTongTouch.ControllerUntouchInteractableObject -= ControllerUntouchInteractableObject;
            }

            if (interactTongTouch != null && state)
            {
                interactTongTouch.ControllerTouchInteractableObject += ControllerTouchInteractableObject;
                interactTongTouch.ControllerUntouchInteractableObject += ControllerUntouchInteractableObject;
            }
        }

        protected override void ControllerTouchInteractableObject(object sender, ObjectInteractEventArgs e)
        {
            if (e.target != null)
            {
                untouchedMaterial = e.target.GetComponent<Renderer>().material;
                e.target.GetComponent<Renderer>().material = touchDebug;
                VRTK_InteractableObject touchedObjectScript = e.target.GetComponent<VRTK_InteractableObject>();
                if (touchedObjectScript != null && touchedObjectScript.useOverrideButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    savedUseButton = subscribedUseButton;
                    useButton = touchedObjectScript.useOverrideButton;
                    ManageUseListener(true);
                }
            }
        }

        protected override void ControllerUntouchInteractableObject(object sender, ObjectInteractEventArgs e)
        {
            if (e.target != null)
            {
                e.target.GetComponent<Renderer>().material = untouchedMaterial;
                VRTK_InteractableObject touchedObjectScript = e.target.GetComponent<VRTK_InteractableObject>();
                if (!touchedObjectScript.IsGrabbed() && savedGrabButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
                {
                    useButton = savedUseButton;
                    savedUseButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
                    ManageUseListener(true);
                }
            }
        }

        protected virtual void ManageUseListener(bool state)
        {
            if (controllerEvents != null && subscribedUseButton != VRTK_ControllerEvents.ButtonAlias.Undefined && (!state || useButton != subscribedUseButton))
            {
                controllerEvents.UnsubscribeToButtonAliasEvent(subscribedUseButton, true, DoUseObject);
                controllerEvents.UnsubscribeToButtonAliasEvent(subscribedUseButton, false, DoUnUseObject);
                subscribedGrabButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
            }

            if (controllerEvents != null && state && useButton != VRTK_ControllerEvents.ButtonAlias.Undefined && useButton != subscribedUseButton)
            {
                controllerEvents.SubscribeToButtonAliasEvent(useButton, true, DoUseObject);
                controllerEvents.SubscribeToButtonAliasEvent(useButton, false, DoUnUseObject);
                subscribedUseButton = useButton;
            }
        }

        protected virtual void ReUseUndroppableObject()
        {
            if (undroppableUsedObject != null)
            {
                VRTK_InteractableObject undroppableUsedObjectScript = undroppableUsedObject.GetComponent<VRTK_InteractableObject>();
                if (interactTongTouch != null && undroppableUsedObjectScript != null && !undroppableUsedObjectScript.IsGrabbed())
                {
                    undroppableUsedObject.SetActive(true);
                    interactTongTouch.ForceTouch(undroppableUsedObject);
                    AttemptGrab();
                }
            }
            else
            {
                undroppableUsedObject = null;
            }
        }

        protected override void SetUndroppableObject()
        {
            if (undroppableUsedObject != null)
            {
                VRTK_InteractableObject undroppableUsedObjectScript = undroppableUsedObject.GetComponent<VRTK_InteractableObject>();
                if (undroppableUsedObjectScript != null && undroppableUsedObjectScript.IsDroppable())
                {
                    undroppableUsedObject = null;
                }
                else
                {
                    undroppableUsedObject.SetActive(false);
                }
            }
        }

        protected override void SetControllerAttachPoint()
        {
            //If no attach point has been specified then just use the tip of the controller
            if (controllerReference.model != null && originalControllerTongAttachPoint == null)
            {
                //attempt to find the attach point on the controller
                SDK_BaseController.ControllerHand handType = VRTK_DeviceFinder.GetControllerHand(interactTongTouch.gameObject);
                string elementPath = VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.AttachPoint, handType);
                Transform defaultAttachPoint = controllerReference.model.transform.Find(elementPath);

                if (defaultAttachPoint != null)
                {
                    controllerAttachPoint = defaultAttachPoint.GetComponent<Rigidbody>();

                    if (controllerAttachPoint == null)
                    {
                        Rigidbody autoGenRB = defaultAttachPoint.gameObject.AddComponent<Rigidbody>();
                        autoGenRB.isKinematic = true;
                        controllerAttachPoint = autoGenRB;
                    }
                }
            }
        }

        protected virtual bool IsObjectUsable(GameObject obj)
        {
            VRTK_InteractableObject objScript = obj.GetComponent<VRTK_InteractableObject>();
            return (interactTongTouch != null && interactTongTouch.IsObjectInteractable(obj) && objScript != null && (objScript.isUsable || objScript.PerformSecondaryAction()));
        }

        protected virtual bool IsObjectHoldOnUse(GameObject obj)
        {
            if (obj != null)
            {
                VRTK_InteractableObject objScript = obj.GetComponent<VRTK_InteractableObject>();
                return (objScript != null && objScript.holdButtonToGrab);
            }
            return false;
        }

        protected virtual void ChooseUseSequence(VRTK_InteractableObject usedObjectScript)
        {
            if (!usedObjectScript.IsGrabbed() || usedObjectScript.IsSwappable())
            {
                InitPrimaryUse(usedObjectScript);
            }
            else
            {
                InitSecondaryGrab(usedObjectScript);
            }
        }

        /* protected virtual void ToggleControllerVisibility(bool visible)
         {
             if (grabbedObject != null)
             {
                 ///[Obsolete]
 #pragma warning disable 0618
                 VRTK_InteractControllerAppearance[] controllerAppearanceScript = grabbedObject.GetComponentsInParent<VRTK_InteractControllerAppearance>(true);
 #pragma warning restore 0618
                 if (controllerAppearanceScript.Length > 0)
                 {
                     controllerAppearanceScript[0].ToggleControllerOnGrab(visible, controllerReference.model, grabbedObject);
                 }
             }
             else if (visible)
             {
                 VRTK_ObjectAppearance.SetRendererVisible(controllerReference.model, grabbedObject);
             }
         } */

        protected virtual void InitUsedObject()
        {
            usedObject = (interactTongTouch != null ? interactTongTouch.GetTouchedObject() : null);
            if (usedObject != null)
            {
                OnControllerStartUseInteractableObject(interactTongTouch.SetControllerInteractEvent(usedObject));
                VRTK_InteractableObject usedObjectScript = usedObject.GetComponent<VRTK_InteractableObject>();
                ChooseUseSequence(usedObjectScript);
               // ToggleControllerVisibility(false);
                OnControllerUseInteractableObject(interactTongTouch.SetControllerInteractEvent(usedObject));
            }
        }

        protected virtual void InitPrimaryUse(VRTK_InteractableObject currentUsedObject)
        {
            if (!currentUsedObject.IsValidInteractableController(gameObject, currentUsedObject.allowedGrabControllers))
            {
                usedObject = null;
                if (interactTongTouch != null && currentUsedObject.IsGrabbed(gameObject))
                {
                    interactTongTouch.ForceStopTouching();
                }
                return;
            }

            influencingUsedObject = false;
            currentUsedObject.SaveCurrentState();
            currentUsedObject.Grabbed(this);
            currentUsedObject.ZeroVelocity();
            currentUsedObject.isKinematic = false;
        }

        protected override void InitSecondaryGrab(VRTK_InteractableObject currentUsedObject)
        {
            influencingGrabbedObject = true;
            currentUsedObject.Grabbed(this);
        }

        protected override void CheckInfluencingObjectOnRelease()
        {
            if (!influencingGrabbedObject && interactTongTouch != null)
            {
                interactTongTouch.ForceStopTouching();
                ToggleControllerVisibility(true);
            }
            influencingGrabbedObject = false;
        }

        protected virtual void InitUnUsedObject(bool applyGrabbingObjectVelocity)
        {
            if (usedObject != null && interactTongTouch != null)
            {
                OnControllerStartUnUseInteractableObject(interactTongTouch.SetControllerInteractEvent(usedObject));
                VRTK_InteractableObject usedObjectScript = usedObject.GetComponent<VRTK_InteractableObject>();
                if (usedObjectScript != null)
                {
                    if (!influencingGrabbedObject)
                    {
                        usedObjectScript.grabAttachMechanicScript.StopGrab(applyGrabbingObjectVelocity);
                    }
                    usedObjectScript.Ungrabbed(this);
                    ToggleControllerVisibility(true);

                    OnControllerUnUseInteractableObject(interactTongTouch.SetControllerInteractEvent(usedObject));
                }
            }

            CheckInfluencingObjectOnRelease();

            useEnabledState = 0;
            usedObject = null;
        }

        protected virtual GameObject GetUsableObject()
        {
            GameObject obj = (interactTongTouch != null ? interactTongTouch.GetTouchedObject() : null);
            if (obj != null && interactTongTouch.IsObjectInteractable(obj))
            {
                return obj;
            }
            return usedObject;
        }

        protected virtual void IncrementUseState()
        {
            if (interactTongTouch != null && !IsObjectHoldOnUse(interactTongTouch.GetTouchedObject()))
            {
                useEnabledState++;
            }
        }


        protected override GameObject GetUndroppableObject()
        {
            if (usedObject != null)
            {
                VRTK_InteractableObject usedObjectScript = usedObject.GetComponent<VRTK_InteractableObject>();
                return (usedObjectScript != null && !usedObjectScript.IsDroppable() ? usedObject : null);
            }
            return null;
        }

        protected virtual void AttemptUseObject()
        {
            GameObject objectToUse = GetUsableObject();
            if (objectToUse != null)
            {
                PerformUseAttempt(objectToUse);
            }
            else
            {
                usePrecognitionTimer = Time.time + usePrecognition;
            }
        }

        protected virtual void PerformUseAttempt(GameObject objectToUse)
        {
            IncrementUseState();
            IsValidUseAttempt(objectToUse);
            undroppableUsedObject = GetUndroppableObject();
        }

        protected virtual bool ScriptValidUse(VRTK_InteractableObject objectToUseScript)
        {
            return (objectToUseScript != null && objectToUseScript.grabAttachMechanicScript != null && objectToUseScript.grabAttachMechanicScript.ValidGrab(controllerTongAttachPoint));
        }

        protected virtual bool IsValidUseAttempt(GameObject objectToUse)
        {
            bool initialGrabAttempt = false;
            VRTK_InteractableObject objectToUseScript = (objectToUse != null ? objectToUse.GetComponent<VRTK_InteractableObject>() : null);
            if (usedObject == null && interactTongTouch != null && IsObjectUsable(interactTongTouch.GetTouchedObject()) && ScriptValidUse(objectToUseScript))
            {
                InitUsedObject();
                if (!influencingUsedObject)
                {
                    initialGrabAttempt = objectToUseScript.grabAttachMechanicScript.StartGrab(gameObject, usedObject, controllerTongAttachPoint);
                }
            }
            return initialGrabAttempt;
        }

        protected virtual bool CanUnUse()
        {
            if (usedObject != null)
            {
                VRTK_InteractableObject objectToUseScript = usedObject.GetComponent<VRTK_InteractableObject>();
                return (objectToUseScript != null && objectToUseScript.IsDroppable());
            }
            return false;
        }

        protected virtual void AttemptUnUseObject()
        {
            if (CanUnUse() && (IsObjectHoldOnGrab(usedObject) || useEnabledState >= 2))
            {
                InitUnUsedObject(true);
            }
        }

        protected virtual void DoUseObject(object sender, ControllerInteractionEventArgs e)
        {
            OnUseButtonPressed(controllerEvents.SetControllerEvent(ref usePressed, true));
            AttemptUseObject();
        }

        protected virtual void DoUnUseObject(object sender, ControllerInteractionEventArgs e)
        {
            AttemptUnUseObject();
            OnUseButtonReleased(controllerEvents.SetControllerEvent(ref usePressed, false));
        }

        protected override void CheckControllerAttachPointSet()
        {
            if (controllerTongAttachPoint == null)
            {
                SetControllerAttachPoint();
            }
        }

        protected override void CreateNonTouchingRigidbody()
        {
            if (createRigidBodyWhenNotTouching && usedObject == null && interactTongTouch != null)
            {
                if (!interactTongTouch.IsRigidBodyForcedActive() && interactTongTouch.IsRigidBodyActive() != usePressed)
                {
                    interactTongTouch.ToggleControllerRigidBody(usePressed);
                }
            }
        }

        protected virtual void CheckPrecognitionUse()
        {
            if (usePrecognitionTimer >= Time.time)
            {
                if (GetUsableObject() != null)
                {
                    AttemptUseObject();
                    if (GetUsedObject() != null)
                    {
                        usePrecognitionTimer = 0f;
                    }
                }
            }
        }

    }
}

