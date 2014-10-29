
GameObject = function() {}

// fields

// properties

/*  Boolean */
Object.defineProperty(GameObject.prototype, "isStatic", 
{
    get: function() { return CS.Call(2, 0, 0, false, this); },
    set: function(v) { return CS.Call(3, 0, 0, false, this, v); }
});

/* ReadOnly Transform */
Object.defineProperty(GameObject.prototype, "transform", 
{
    get: function() { return CS.Call(2, 0, 1, false, this); },
    set: function(v) { return CS.Call(3, 0, 1, false, this, v); }
});

/* ReadOnly Rigidbody */
Object.defineProperty(GameObject.prototype, "rigidbody", 
{
    get: function() { return CS.Call(2, 0, 2, false, this); },
    set: function(v) { return CS.Call(3, 0, 2, false, this, v); }
});

/* ReadOnly Rigidbody2D */
Object.defineProperty(GameObject.prototype, "rigidbody2D", 
{
    get: function() { return CS.Call(2, 0, 3, false, this); },
    set: function(v) { return CS.Call(3, 0, 3, false, this, v); }
});

/* ReadOnly Camera */
Object.defineProperty(GameObject.prototype, "camera", 
{
    get: function() { return CS.Call(2, 0, 4, false, this); },
    set: function(v) { return CS.Call(3, 0, 4, false, this, v); }
});

/* ReadOnly Light */
Object.defineProperty(GameObject.prototype, "light", 
{
    get: function() { return CS.Call(2, 0, 5, false, this); },
    set: function(v) { return CS.Call(3, 0, 5, false, this, v); }
});

/* ReadOnly Animation */
Object.defineProperty(GameObject.prototype, "animation", 
{
    get: function() { return CS.Call(2, 0, 6, false, this); },
    set: function(v) { return CS.Call(3, 0, 6, false, this, v); }
});

/* ReadOnly ConstantForce */
Object.defineProperty(GameObject.prototype, "constantForce", 
{
    get: function() { return CS.Call(2, 0, 7, false, this); },
    set: function(v) { return CS.Call(3, 0, 7, false, this, v); }
});

/* ReadOnly Renderer */
Object.defineProperty(GameObject.prototype, "renderer", 
{
    get: function() { return CS.Call(2, 0, 8, false, this); },
    set: function(v) { return CS.Call(3, 0, 8, false, this, v); }
});

/* ReadOnly AudioSource */
Object.defineProperty(GameObject.prototype, "audio", 
{
    get: function() { return CS.Call(2, 0, 9, false, this); },
    set: function(v) { return CS.Call(3, 0, 9, false, this, v); }
});

/* ReadOnly GUIText */
Object.defineProperty(GameObject.prototype, "guiText", 
{
    get: function() { return CS.Call(2, 0, 10, false, this); },
    set: function(v) { return CS.Call(3, 0, 10, false, this, v); }
});

/* ReadOnly NetworkView */
Object.defineProperty(GameObject.prototype, "networkView", 
{
    get: function() { return CS.Call(2, 0, 11, false, this); },
    set: function(v) { return CS.Call(3, 0, 11, false, this, v); }
});

/* ReadOnly GUIElement */
Object.defineProperty(GameObject.prototype, "guiElement", 
{
    get: function() { return CS.Call(2, 0, 12, false, this); },
    set: function(v) { return CS.Call(3, 0, 12, false, this, v); }
});

/* ReadOnly GUITexture */
Object.defineProperty(GameObject.prototype, "guiTexture", 
{
    get: function() { return CS.Call(2, 0, 13, false, this); },
    set: function(v) { return CS.Call(3, 0, 13, false, this, v); }
});

/* ReadOnly Collider */
Object.defineProperty(GameObject.prototype, "collider", 
{
    get: function() { return CS.Call(2, 0, 14, false, this); },
    set: function(v) { return CS.Call(3, 0, 14, false, this, v); }
});

/* ReadOnly Collider2D */
Object.defineProperty(GameObject.prototype, "collider2D", 
{
    get: function() { return CS.Call(2, 0, 15, false, this); },
    set: function(v) { return CS.Call(3, 0, 15, false, this, v); }
});

/* ReadOnly HingeJoint */
Object.defineProperty(GameObject.prototype, "hingeJoint", 
{
    get: function() { return CS.Call(2, 0, 16, false, this); },
    set: function(v) { return CS.Call(3, 0, 16, false, this, v); }
});

/* ReadOnly ParticleEmitter */
Object.defineProperty(GameObject.prototype, "particleEmitter", 
{
    get: function() { return CS.Call(2, 0, 17, false, this); },
    set: function(v) { return CS.Call(3, 0, 17, false, this, v); }
});

/* ReadOnly ParticleSystem */
Object.defineProperty(GameObject.prototype, "particleSystem", 
{
    get: function() { return CS.Call(2, 0, 18, false, this); },
    set: function(v) { return CS.Call(3, 0, 18, false, this, v); }
});

/*  Int32 */
Object.defineProperty(GameObject.prototype, "layer", 
{
    get: function() { return CS.Call(2, 0, 19, false, this); },
    set: function(v) { return CS.Call(3, 0, 19, false, this, v); }
});

/*  Boolean */
Object.defineProperty(GameObject.prototype, "active", 
{
    get: function() { return CS.Call(2, 0, 20, false, this); },
    set: function(v) { return CS.Call(3, 0, 20, false, this, v); }
});

/* ReadOnly Boolean */
Object.defineProperty(GameObject.prototype, "activeSelf", 
{
    get: function() { return CS.Call(2, 0, 21, false, this); },
    set: function(v) { return CS.Call(3, 0, 21, false, this, v); }
});

/* ReadOnly Boolean */
Object.defineProperty(GameObject.prototype, "activeInHierarchy", 
{
    get: function() { return CS.Call(2, 0, 22, false, this); },
    set: function(v) { return CS.Call(3, 0, 22, false, this, v); }
});

/*  String */
Object.defineProperty(GameObject.prototype, "tag", 
{
    get: function() { return CS.Call(2, 0, 23, false, this); },
    set: function(v) { return CS.Call(3, 0, 23, false, this, v); }
});

/* ReadOnly GameObject */
Object.defineProperty(GameObject.prototype, "gameObject", 
{
    get: function() { return CS.Call(2, 0, 24, false, this); },
    set: function(v) { return CS.Call(3, 0, 24, false, this, v); }
});

/*  String */
Object.defineProperty(GameObject.prototype, "name", 
{
    get: function() { return CS.Call(2, 0, 25, false, this); },
    set: function(v) { return CS.Call(3, 0, 25, false, this, v); }
});

/*  HideFlags */
Object.defineProperty(GameObject.prototype, "hideFlags", 
{
    get: function() { return CS.Call(2, 0, 26, false, this); },
    set: function(v) { return CS.Call(3, 0, 26, false, this, v); }
});

// methods

/* Void */
GameObject.prototype.SampleAnimation = function(arg0/* AnimationClip */, arg1/* Single */) { return CS.Call(4, 0, 0, false, this, arg0,arg1); }
/* static GameObject */
GameObject.CreatePrimitive = function(arg0/* PrimitiveType */) { return CS.Call(4, 0, 1, true, arg0); }
/* Component */
GameObject.prototype.GetComponent = function(arg0/* Type */) { return CS.Call(4, 0, 2, false, this, arg0); }
/* T */
GameObject.prototype.GetComponent = function() { return CS.Call(4, 0, 3, false, this); }
/* Component */
GameObject.prototype.GetComponent = function(arg0/* String */) { return CS.Call(4, 0, 4, false, this, arg0); }
/* Component */
GameObject.prototype.GetComponentInChildren = function(arg0/* Type */) { return CS.Call(4, 0, 5, false, this, arg0); }
/* T */
GameObject.prototype.GetComponentInChildren = function() { return CS.Call(4, 0, 6, false, this); }
/* Component */
GameObject.prototype.GetComponentInParent = function(arg0/* Type */) { return CS.Call(4, 0, 7, false, this, arg0); }
/* T */
GameObject.prototype.GetComponentInParent = function() { return CS.Call(4, 0, 8, false, this); }
/* Boolean */
GameObject.prototype.get_isStatic = function() { return CS.Call(4, 0, 9, false, this); }
/* Void */
GameObject.prototype.set_isStatic = function(arg0/* Boolean */) { return CS.Call(4, 0, 10, false, this, arg0); }
/* Component[] */
GameObject.prototype.GetComponents = function(arg0/* Type */) { return CS.Call(4, 0, 11, false, this, arg0); }
/* T[] */
GameObject.prototype.GetComponents = function() { return CS.Call(4, 0, 12, false, this); }
/* Component[] */
GameObject.prototype.GetComponentsInChildren = function(arg0/* Type */) { return CS.Call(4, 0, 13, false, this, arg0); }
/* Component[] */
GameObject.prototype.GetComponentsInChildren = function(arg0/* Type */, arg1/* Boolean */) { return CS.Call(4, 0, 14, false, this, arg0,arg1); }
/* T[] */
GameObject.prototype.GetComponentsInChildren = function(arg0/* Boolean */) { return CS.Call(4, 0, 15, false, this, arg0); }
/* T[] */
GameObject.prototype.GetComponentsInChildren = function() { return CS.Call(4, 0, 16, false, this); }
/* Component[] */
GameObject.prototype.GetComponentsInParent = function(arg0/* Type */) { return CS.Call(4, 0, 17, false, this, arg0); }
/* Component[] */
GameObject.prototype.GetComponentsInParent = function(arg0/* Type */, arg1/* Boolean */) { return CS.Call(4, 0, 18, false, this, arg0,arg1); }
/* T[] */
GameObject.prototype.GetComponentsInParent = function(arg0/* Boolean */) { return CS.Call(4, 0, 19, false, this, arg0); }
/* T[] */
GameObject.prototype.GetComponentsInParent = function() { return CS.Call(4, 0, 20, false, this); }
/* Transform */
GameObject.prototype.get_transform = function() { return CS.Call(4, 0, 21, false, this); }
/* Rigidbody */
GameObject.prototype.get_rigidbody = function() { return CS.Call(4, 0, 22, false, this); }
/* Rigidbody2D */
GameObject.prototype.get_rigidbody2D = function() { return CS.Call(4, 0, 23, false, this); }
/* Camera */
GameObject.prototype.get_camera = function() { return CS.Call(4, 0, 24, false, this); }
/* Light */
GameObject.prototype.get_light = function() { return CS.Call(4, 0, 25, false, this); }
/* Animation */
GameObject.prototype.get_animation = function() { return CS.Call(4, 0, 26, false, this); }
/* ConstantForce */
GameObject.prototype.get_constantForce = function() { return CS.Call(4, 0, 27, false, this); }
/* Renderer */
GameObject.prototype.get_renderer = function() { return CS.Call(4, 0, 28, false, this); }
/* AudioSource */
GameObject.prototype.get_audio = function() { return CS.Call(4, 0, 29, false, this); }
/* GUIText */
GameObject.prototype.get_guiText = function() { return CS.Call(4, 0, 30, false, this); }
/* NetworkView */
GameObject.prototype.get_networkView = function() { return CS.Call(4, 0, 31, false, this); }
/* GUIElement */
GameObject.prototype.get_guiElement = function() { return CS.Call(4, 0, 32, false, this); }
/* GUITexture */
GameObject.prototype.get_guiTexture = function() { return CS.Call(4, 0, 33, false, this); }
/* Collider */
GameObject.prototype.get_collider = function() { return CS.Call(4, 0, 34, false, this); }
/* Collider2D */
GameObject.prototype.get_collider2D = function() { return CS.Call(4, 0, 35, false, this); }
/* HingeJoint */
GameObject.prototype.get_hingeJoint = function() { return CS.Call(4, 0, 36, false, this); }
/* ParticleEmitter */
GameObject.prototype.get_particleEmitter = function() { return CS.Call(4, 0, 37, false, this); }
/* ParticleSystem */
GameObject.prototype.get_particleSystem = function() { return CS.Call(4, 0, 38, false, this); }
/* Int32 */
GameObject.prototype.get_layer = function() { return CS.Call(4, 0, 39, false, this); }
/* Void */
GameObject.prototype.set_layer = function(arg0/* Int32 */) { return CS.Call(4, 0, 40, false, this, arg0); }
/* Boolean */
GameObject.prototype.get_active = function() { return CS.Call(4, 0, 41, false, this); }
/* Void */
GameObject.prototype.set_active = function(arg0/* Boolean */) { return CS.Call(4, 0, 42, false, this, arg0); }
/* Void */
GameObject.prototype.SetActive = function(arg0/* Boolean */) { return CS.Call(4, 0, 43, false, this, arg0); }
/* Boolean */
GameObject.prototype.get_activeSelf = function() { return CS.Call(4, 0, 44, false, this); }
/* Boolean */
GameObject.prototype.get_activeInHierarchy = function() { return CS.Call(4, 0, 45, false, this); }
/* Void */
GameObject.prototype.SetActiveRecursively = function(arg0/* Boolean */) { return CS.Call(4, 0, 46, false, this, arg0); }
/* String */
GameObject.prototype.get_tag = function() { return CS.Call(4, 0, 47, false, this); }
/* Void */
GameObject.prototype.set_tag = function(arg0/* String */) { return CS.Call(4, 0, 48, false, this, arg0); }
/* Boolean */
GameObject.prototype.CompareTag = function(arg0/* String */) { return CS.Call(4, 0, 49, false, this, arg0); }
/* static GameObject */
GameObject.FindGameObjectWithTag = function(arg0/* String */) { return CS.Call(4, 0, 50, true, arg0); }
/* static GameObject */
GameObject.FindWithTag = function(arg0/* String */) { return CS.Call(4, 0, 51, true, arg0); }
/* static GameObject[] */
GameObject.FindGameObjectsWithTag = function(arg0/* String */) { return CS.Call(4, 0, 52, true, arg0); }
/* Void */
GameObject.prototype.SendMessageUpwards = function(arg0/* String */, arg1/* Object */, arg2/* SendMessageOptions */) { return CS.Call(4, 0, 53, false, this, arg0,arg1,arg2); }
/* Void */
GameObject.prototype.SendMessageUpwards = function(arg0/* String */, arg1/* Object */) { return CS.Call(4, 0, 54, false, this, arg0,arg1); }
/* Void */
GameObject.prototype.SendMessageUpwards = function(arg0/* String */) { return CS.Call(4, 0, 55, false, this, arg0); }
/* Void */
GameObject.prototype.SendMessageUpwards = function(arg0/* String */, arg1/* SendMessageOptions */) { return CS.Call(4, 0, 56, false, this, arg0,arg1); }
/* Void */
GameObject.prototype.SendMessage = function(arg0/* String */, arg1/* Object */, arg2/* SendMessageOptions */) { return CS.Call(4, 0, 57, false, this, arg0,arg1,arg2); }
/* Void */
GameObject.prototype.SendMessage = function(arg0/* String */, arg1/* Object */) { return CS.Call(4, 0, 58, false, this, arg0,arg1); }
/* Void */
GameObject.prototype.SendMessage = function(arg0/* String */) { return CS.Call(4, 0, 59, false, this, arg0); }
/* Void */
GameObject.prototype.SendMessage = function(arg0/* String */, arg1/* SendMessageOptions */) { return CS.Call(4, 0, 60, false, this, arg0,arg1); }
/* Void */
GameObject.prototype.BroadcastMessage = function(arg0/* String */, arg1/* Object */, arg2/* SendMessageOptions */) { return CS.Call(4, 0, 61, false, this, arg0,arg1,arg2); }
/* Void */
GameObject.prototype.BroadcastMessage = function(arg0/* String */, arg1/* Object */) { return CS.Call(4, 0, 62, false, this, arg0,arg1); }
/* Void */
GameObject.prototype.BroadcastMessage = function(arg0/* String */) { return CS.Call(4, 0, 63, false, this, arg0); }
/* Void */
GameObject.prototype.BroadcastMessage = function(arg0/* String */, arg1/* SendMessageOptions */) { return CS.Call(4, 0, 64, false, this, arg0,arg1); }
/* Component */
GameObject.prototype.AddComponent = function(arg0/* String */) { return CS.Call(4, 0, 65, false, this, arg0); }
/* Component */
GameObject.prototype.AddComponent = function(arg0/* Type */) { return CS.Call(4, 0, 66, false, this, arg0); }
/* T */
GameObject.prototype.AddComponent = function() { return CS.Call(4, 0, 67, false, this); }
/* Void */
GameObject.prototype.PlayAnimation = function(arg0/* AnimationClip */) { return CS.Call(4, 0, 68, false, this, arg0); }
/* Void */
GameObject.prototype.StopAnimation = function() { return CS.Call(4, 0, 69, false, this); }
/* static GameObject */
GameObject.Find = function(arg0/* String */) { return CS.Call(4, 0, 70, true, arg0); }
/* GameObject */
GameObject.prototype.get_gameObject = function() { return CS.Call(4, 0, 71, false, this); }
/* Boolean */
GameObject.prototype.Equals = function(arg0/* Object */) { return CS.Call(4, 0, 72, false, this, arg0); }
/* Int32 */
GameObject.prototype.GetHashCode = function() { return CS.Call(4, 0, 73, false, this); }
/* Int32 */
GameObject.prototype.GetInstanceID = function() { return CS.Call(4, 0, 74, false, this); }
/* String */
GameObject.prototype.get_name = function() { return CS.Call(4, 0, 75, false, this); }
/* Void */
GameObject.prototype.set_name = function(arg0/* String */) { return CS.Call(4, 0, 76, false, this, arg0); }
/* HideFlags */
GameObject.prototype.get_hideFlags = function() { return CS.Call(4, 0, 77, false, this); }
/* Void */
GameObject.prototype.set_hideFlags = function(arg0/* HideFlags */) { return CS.Call(4, 0, 78, false, this, arg0); }
/* String */
GameObject.prototype.ToString = function() { return CS.Call(4, 0, 79, false, this); }
/* Type */
GameObject.prototype.GetType = function() { return CS.Call(4, 0, 80, false, this); }
