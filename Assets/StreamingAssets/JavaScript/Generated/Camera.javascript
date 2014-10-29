
Camera = function() {}

// fields

// properties

/*  Single */
Object.defineProperty(Camera.prototype, "fov", 
{
    get: function() { return CS.Call(2, 1, 0, false, this); },
    set: function(v) { return CS.Call(3, 1, 0, false, this, v); }
});

/*  Single */
Object.defineProperty(Camera.prototype, "near", 
{
    get: function() { return CS.Call(2, 1, 1, false, this); },
    set: function(v) { return CS.Call(3, 1, 1, false, this, v); }
});

/*  Single */
Object.defineProperty(Camera.prototype, "far", 
{
    get: function() { return CS.Call(2, 1, 2, false, this); },
    set: function(v) { return CS.Call(3, 1, 2, false, this, v); }
});

/*  Single */
Object.defineProperty(Camera.prototype, "fieldOfView", 
{
    get: function() { return CS.Call(2, 1, 3, false, this); },
    set: function(v) { return CS.Call(3, 1, 3, false, this, v); }
});

/*  Single */
Object.defineProperty(Camera.prototype, "nearClipPlane", 
{
    get: function() { return CS.Call(2, 1, 4, false, this); },
    set: function(v) { return CS.Call(3, 1, 4, false, this, v); }
});

/*  Single */
Object.defineProperty(Camera.prototype, "farClipPlane", 
{
    get: function() { return CS.Call(2, 1, 5, false, this); },
    set: function(v) { return CS.Call(3, 1, 5, false, this, v); }
});

/*  RenderingPath */
Object.defineProperty(Camera.prototype, "renderingPath", 
{
    get: function() { return CS.Call(2, 1, 6, false, this); },
    set: function(v) { return CS.Call(3, 1, 6, false, this, v); }
});

/* ReadOnly RenderingPath */
Object.defineProperty(Camera.prototype, "actualRenderingPath", 
{
    get: function() { return CS.Call(2, 1, 7, false, this); },
    set: function(v) { return CS.Call(3, 1, 7, false, this, v); }
});

/*  Boolean */
Object.defineProperty(Camera.prototype, "hdr", 
{
    get: function() { return CS.Call(2, 1, 8, false, this); },
    set: function(v) { return CS.Call(3, 1, 8, false, this, v); }
});

/*  Single */
Object.defineProperty(Camera.prototype, "orthographicSize", 
{
    get: function() { return CS.Call(2, 1, 9, false, this); },
    set: function(v) { return CS.Call(3, 1, 9, false, this, v); }
});

/*  Boolean */
Object.defineProperty(Camera.prototype, "orthographic", 
{
    get: function() { return CS.Call(2, 1, 10, false, this); },
    set: function(v) { return CS.Call(3, 1, 10, false, this, v); }
});

/*  TransparencySortMode */
Object.defineProperty(Camera.prototype, "transparencySortMode", 
{
    get: function() { return CS.Call(2, 1, 11, false, this); },
    set: function(v) { return CS.Call(3, 1, 11, false, this, v); }
});

/*  Boolean */
Object.defineProperty(Camera.prototype, "isOrthoGraphic", 
{
    get: function() { return CS.Call(2, 1, 12, false, this); },
    set: function(v) { return CS.Call(3, 1, 12, false, this, v); }
});

/*  Single */
Object.defineProperty(Camera.prototype, "depth", 
{
    get: function() { return CS.Call(2, 1, 13, false, this); },
    set: function(v) { return CS.Call(3, 1, 13, false, this, v); }
});

/*  Single */
Object.defineProperty(Camera.prototype, "aspect", 
{
    get: function() { return CS.Call(2, 1, 14, false, this); },
    set: function(v) { return CS.Call(3, 1, 14, false, this, v); }
});

/*  Int32 */
Object.defineProperty(Camera.prototype, "cullingMask", 
{
    get: function() { return CS.Call(2, 1, 15, false, this); },
    set: function(v) { return CS.Call(3, 1, 15, false, this, v); }
});

/*  Int32 */
Object.defineProperty(Camera.prototype, "eventMask", 
{
    get: function() { return CS.Call(2, 1, 16, false, this); },
    set: function(v) { return CS.Call(3, 1, 16, false, this, v); }
});

/*  Color */
Object.defineProperty(Camera.prototype, "backgroundColor", 
{
    get: function() { return CS.Call(2, 1, 17, false, this); },
    set: function(v) { return CS.Call(3, 1, 17, false, this, v); }
});

/*  Rect */
Object.defineProperty(Camera.prototype, "rect", 
{
    get: function() { return CS.Call(2, 1, 18, false, this); },
    set: function(v) { return CS.Call(3, 1, 18, false, this, v); }
});

/*  Rect */
Object.defineProperty(Camera.prototype, "pixelRect", 
{
    get: function() { return CS.Call(2, 1, 19, false, this); },
    set: function(v) { return CS.Call(3, 1, 19, false, this, v); }
});

/*  RenderTexture */
Object.defineProperty(Camera.prototype, "targetTexture", 
{
    get: function() { return CS.Call(2, 1, 20, false, this); },
    set: function(v) { return CS.Call(3, 1, 20, false, this, v); }
});

/* ReadOnly Single */
Object.defineProperty(Camera.prototype, "pixelWidth", 
{
    get: function() { return CS.Call(2, 1, 21, false, this); },
    set: function(v) { return CS.Call(3, 1, 21, false, this, v); }
});

/* ReadOnly Single */
Object.defineProperty(Camera.prototype, "pixelHeight", 
{
    get: function() { return CS.Call(2, 1, 22, false, this); },
    set: function(v) { return CS.Call(3, 1, 22, false, this, v); }
});

/* ReadOnly Matrix4x4 */
Object.defineProperty(Camera.prototype, "cameraToWorldMatrix", 
{
    get: function() { return CS.Call(2, 1, 23, false, this); },
    set: function(v) { return CS.Call(3, 1, 23, false, this, v); }
});

/*  Matrix4x4 */
Object.defineProperty(Camera.prototype, "worldToCameraMatrix", 
{
    get: function() { return CS.Call(2, 1, 24, false, this); },
    set: function(v) { return CS.Call(3, 1, 24, false, this, v); }
});

/*  Matrix4x4 */
Object.defineProperty(Camera.prototype, "projectionMatrix", 
{
    get: function() { return CS.Call(2, 1, 25, false, this); },
    set: function(v) { return CS.Call(3, 1, 25, false, this, v); }
});

/* ReadOnly Vector3 */
Object.defineProperty(Camera.prototype, "velocity", 
{
    get: function() { return CS.Call(2, 1, 26, false, this); },
    set: function(v) { return CS.Call(3, 1, 26, false, this, v); }
});

/*  CameraClearFlags */
Object.defineProperty(Camera.prototype, "clearFlags", 
{
    get: function() { return CS.Call(2, 1, 27, false, this); },
    set: function(v) { return CS.Call(3, 1, 27, false, this, v); }
});

/* ReadOnly Boolean */
Object.defineProperty(Camera.prototype, "stereoEnabled", 
{
    get: function() { return CS.Call(2, 1, 28, false, this); },
    set: function(v) { return CS.Call(3, 1, 28, false, this, v); }
});

/*  Single */
Object.defineProperty(Camera.prototype, "stereoSeparation", 
{
    get: function() { return CS.Call(2, 1, 29, false, this); },
    set: function(v) { return CS.Call(3, 1, 29, false, this, v); }
});

/*  Single */
Object.defineProperty(Camera.prototype, "stereoConvergence", 
{
    get: function() { return CS.Call(2, 1, 30, false, this); },
    set: function(v) { return CS.Call(3, 1, 30, false, this, v); }
});

/*  Boolean */
Object.defineProperty(Camera.prototype, "useOcclusionCulling", 
{
    get: function() { return CS.Call(2, 1, 31, false, this); },
    set: function(v) { return CS.Call(3, 1, 31, false, this, v); }
});

/*  Single[] */
Object.defineProperty(Camera.prototype, "layerCullDistances", 
{
    get: function() { return CS.Call(2, 1, 32, false, this); },
    set: function(v) { return CS.Call(3, 1, 32, false, this, v); }
});

/*  Boolean */
Object.defineProperty(Camera.prototype, "layerCullSpherical", 
{
    get: function() { return CS.Call(2, 1, 33, false, this); },
    set: function(v) { return CS.Call(3, 1, 33, false, this, v); }
});

/*  DepthTextureMode */
Object.defineProperty(Camera.prototype, "depthTextureMode", 
{
    get: function() { return CS.Call(2, 1, 34, false, this); },
    set: function(v) { return CS.Call(3, 1, 34, false, this, v); }
});

/*  Boolean */
Object.defineProperty(Camera.prototype, "clearStencilAfterLightingPass", 
{
    get: function() { return CS.Call(2, 1, 35, false, this); },
    set: function(v) { return CS.Call(3, 1, 35, false, this, v); }
});

/*  Boolean */
Object.defineProperty(Camera.prototype, "enabled", 
{
    get: function() { return CS.Call(2, 1, 36, false, this); },
    set: function(v) { return CS.Call(3, 1, 36, false, this, v); }
});

/* ReadOnly Transform */
Object.defineProperty(Camera.prototype, "transform", 
{
    get: function() { return CS.Call(2, 1, 37, false, this); },
    set: function(v) { return CS.Call(3, 1, 37, false, this, v); }
});

/* ReadOnly Rigidbody */
Object.defineProperty(Camera.prototype, "rigidbody", 
{
    get: function() { return CS.Call(2, 1, 38, false, this); },
    set: function(v) { return CS.Call(3, 1, 38, false, this, v); }
});

/* ReadOnly Rigidbody2D */
Object.defineProperty(Camera.prototype, "rigidbody2D", 
{
    get: function() { return CS.Call(2, 1, 39, false, this); },
    set: function(v) { return CS.Call(3, 1, 39, false, this, v); }
});

/* ReadOnly Camera */
Object.defineProperty(Camera.prototype, "camera", 
{
    get: function() { return CS.Call(2, 1, 40, false, this); },
    set: function(v) { return CS.Call(3, 1, 40, false, this, v); }
});

/* ReadOnly Light */
Object.defineProperty(Camera.prototype, "light", 
{
    get: function() { return CS.Call(2, 1, 41, false, this); },
    set: function(v) { return CS.Call(3, 1, 41, false, this, v); }
});

/* ReadOnly Animation */
Object.defineProperty(Camera.prototype, "animation", 
{
    get: function() { return CS.Call(2, 1, 42, false, this); },
    set: function(v) { return CS.Call(3, 1, 42, false, this, v); }
});

/* ReadOnly ConstantForce */
Object.defineProperty(Camera.prototype, "constantForce", 
{
    get: function() { return CS.Call(2, 1, 43, false, this); },
    set: function(v) { return CS.Call(3, 1, 43, false, this, v); }
});

/* ReadOnly Renderer */
Object.defineProperty(Camera.prototype, "renderer", 
{
    get: function() { return CS.Call(2, 1, 44, false, this); },
    set: function(v) { return CS.Call(3, 1, 44, false, this, v); }
});

/* ReadOnly AudioSource */
Object.defineProperty(Camera.prototype, "audio", 
{
    get: function() { return CS.Call(2, 1, 45, false, this); },
    set: function(v) { return CS.Call(3, 1, 45, false, this, v); }
});

/* ReadOnly GUIText */
Object.defineProperty(Camera.prototype, "guiText", 
{
    get: function() { return CS.Call(2, 1, 46, false, this); },
    set: function(v) { return CS.Call(3, 1, 46, false, this, v); }
});

/* ReadOnly NetworkView */
Object.defineProperty(Camera.prototype, "networkView", 
{
    get: function() { return CS.Call(2, 1, 47, false, this); },
    set: function(v) { return CS.Call(3, 1, 47, false, this, v); }
});

/* ReadOnly GUIElement */
Object.defineProperty(Camera.prototype, "guiElement", 
{
    get: function() { return CS.Call(2, 1, 48, false, this); },
    set: function(v) { return CS.Call(3, 1, 48, false, this, v); }
});

/* ReadOnly GUITexture */
Object.defineProperty(Camera.prototype, "guiTexture", 
{
    get: function() { return CS.Call(2, 1, 49, false, this); },
    set: function(v) { return CS.Call(3, 1, 49, false, this, v); }
});

/* ReadOnly Collider */
Object.defineProperty(Camera.prototype, "collider", 
{
    get: function() { return CS.Call(2, 1, 50, false, this); },
    set: function(v) { return CS.Call(3, 1, 50, false, this, v); }
});

/* ReadOnly Collider2D */
Object.defineProperty(Camera.prototype, "collider2D", 
{
    get: function() { return CS.Call(2, 1, 51, false, this); },
    set: function(v) { return CS.Call(3, 1, 51, false, this, v); }
});

/* ReadOnly HingeJoint */
Object.defineProperty(Camera.prototype, "hingeJoint", 
{
    get: function() { return CS.Call(2, 1, 52, false, this); },
    set: function(v) { return CS.Call(3, 1, 52, false, this, v); }
});

/* ReadOnly ParticleEmitter */
Object.defineProperty(Camera.prototype, "particleEmitter", 
{
    get: function() { return CS.Call(2, 1, 53, false, this); },
    set: function(v) { return CS.Call(3, 1, 53, false, this, v); }
});

/* ReadOnly ParticleSystem */
Object.defineProperty(Camera.prototype, "particleSystem", 
{
    get: function() { return CS.Call(2, 1, 54, false, this); },
    set: function(v) { return CS.Call(3, 1, 54, false, this, v); }
});

/* ReadOnly GameObject */
Object.defineProperty(Camera.prototype, "gameObject", 
{
    get: function() { return CS.Call(2, 1, 55, false, this); },
    set: function(v) { return CS.Call(3, 1, 55, false, this, v); }
});

/*  Boolean */
Object.defineProperty(Camera.prototype, "active", 
{
    get: function() { return CS.Call(2, 1, 56, false, this); },
    set: function(v) { return CS.Call(3, 1, 56, false, this, v); }
});

/*  String */
Object.defineProperty(Camera.prototype, "tag", 
{
    get: function() { return CS.Call(2, 1, 57, false, this); },
    set: function(v) { return CS.Call(3, 1, 57, false, this, v); }
});

/*  String */
Object.defineProperty(Camera.prototype, "name", 
{
    get: function() { return CS.Call(2, 1, 58, false, this); },
    set: function(v) { return CS.Call(3, 1, 58, false, this, v); }
});

/*  HideFlags */
Object.defineProperty(Camera.prototype, "hideFlags", 
{
    get: function() { return CS.Call(2, 1, 59, false, this); },
    set: function(v) { return CS.Call(3, 1, 59, false, this, v); }
});

// methods

/* Single */
Camera.prototype.get_fov = function() { return CS.Call(4, 1, 0, false, this); }
/* Void */
Camera.prototype.set_fov = function(arg0/* Single */) { return CS.Call(4, 1, 1, false, this, arg0); }
/* Single */
Camera.prototype.get_near = function() { return CS.Call(4, 1, 2, false, this); }
/* Void */
Camera.prototype.set_near = function(arg0/* Single */) { return CS.Call(4, 1, 3, false, this, arg0); }
/* Single */
Camera.prototype.get_far = function() { return CS.Call(4, 1, 4, false, this); }
/* Void */
Camera.prototype.set_far = function(arg0/* Single */) { return CS.Call(4, 1, 5, false, this, arg0); }
/* Single */
Camera.prototype.get_fieldOfView = function() { return CS.Call(4, 1, 6, false, this); }
/* Void */
Camera.prototype.set_fieldOfView = function(arg0/* Single */) { return CS.Call(4, 1, 7, false, this, arg0); }
/* Single */
Camera.prototype.get_nearClipPlane = function() { return CS.Call(4, 1, 8, false, this); }
/* Void */
Camera.prototype.set_nearClipPlane = function(arg0/* Single */) { return CS.Call(4, 1, 9, false, this, arg0); }
/* Single */
Camera.prototype.get_farClipPlane = function() { return CS.Call(4, 1, 10, false, this); }
/* Void */
Camera.prototype.set_farClipPlane = function(arg0/* Single */) { return CS.Call(4, 1, 11, false, this, arg0); }
/* RenderingPath */
Camera.prototype.get_renderingPath = function() { return CS.Call(4, 1, 12, false, this); }
/* Void */
Camera.prototype.set_renderingPath = function(arg0/* RenderingPath */) { return CS.Call(4, 1, 13, false, this, arg0); }
/* RenderingPath */
Camera.prototype.get_actualRenderingPath = function() { return CS.Call(4, 1, 14, false, this); }
/* Boolean */
Camera.prototype.get_hdr = function() { return CS.Call(4, 1, 15, false, this); }
/* Void */
Camera.prototype.set_hdr = function(arg0/* Boolean */) { return CS.Call(4, 1, 16, false, this, arg0); }
/* Single */
Camera.prototype.get_orthographicSize = function() { return CS.Call(4, 1, 17, false, this); }
/* Void */
Camera.prototype.set_orthographicSize = function(arg0/* Single */) { return CS.Call(4, 1, 18, false, this, arg0); }
/* Boolean */
Camera.prototype.get_orthographic = function() { return CS.Call(4, 1, 19, false, this); }
/* Void */
Camera.prototype.set_orthographic = function(arg0/* Boolean */) { return CS.Call(4, 1, 20, false, this, arg0); }
/* TransparencySortMode */
Camera.prototype.get_transparencySortMode = function() { return CS.Call(4, 1, 21, false, this); }
/* Void */
Camera.prototype.set_transparencySortMode = function(arg0/* TransparencySortMode */) { return CS.Call(4, 1, 22, false, this, arg0); }
/* Boolean */
Camera.prototype.get_isOrthoGraphic = function() { return CS.Call(4, 1, 23, false, this); }
/* Void */
Camera.prototype.set_isOrthoGraphic = function(arg0/* Boolean */) { return CS.Call(4, 1, 24, false, this, arg0); }
/* Single */
Camera.prototype.get_depth = function() { return CS.Call(4, 1, 25, false, this); }
/* Void */
Camera.prototype.set_depth = function(arg0/* Single */) { return CS.Call(4, 1, 26, false, this, arg0); }
/* Single */
Camera.prototype.get_aspect = function() { return CS.Call(4, 1, 27, false, this); }
/* Void */
Camera.prototype.set_aspect = function(arg0/* Single */) { return CS.Call(4, 1, 28, false, this, arg0); }
/* Int32 */
Camera.prototype.get_cullingMask = function() { return CS.Call(4, 1, 29, false, this); }
/* Void */
Camera.prototype.set_cullingMask = function(arg0/* Int32 */) { return CS.Call(4, 1, 30, false, this, arg0); }
/* Int32 */
Camera.prototype.get_eventMask = function() { return CS.Call(4, 1, 31, false, this); }
/* Void */
Camera.prototype.set_eventMask = function(arg0/* Int32 */) { return CS.Call(4, 1, 32, false, this, arg0); }
/* Color */
Camera.prototype.get_backgroundColor = function() { return CS.Call(4, 1, 33, false, this); }
/* Void */
Camera.prototype.set_backgroundColor = function(arg0/* Color */) { return CS.Call(4, 1, 34, false, this, arg0); }
/* Rect */
Camera.prototype.get_rect = function() { return CS.Call(4, 1, 35, false, this); }
/* Void */
Camera.prototype.set_rect = function(arg0/* Rect */) { return CS.Call(4, 1, 36, false, this, arg0); }
/* Rect */
Camera.prototype.get_pixelRect = function() { return CS.Call(4, 1, 37, false, this); }
/* Void */
Camera.prototype.set_pixelRect = function(arg0/* Rect */) { return CS.Call(4, 1, 38, false, this, arg0); }
/* RenderTexture */
Camera.prototype.get_targetTexture = function() { return CS.Call(4, 1, 39, false, this); }
/* Void */
Camera.prototype.set_targetTexture = function(arg0/* RenderTexture */) { return CS.Call(4, 1, 40, false, this, arg0); }
/* Void */
Camera.prototype.SetTargetBuffers = function(arg0/* RenderBuffer */, arg1/* RenderBuffer */) { return CS.Call(4, 1, 41, false, this, arg0,arg1); }
/* Void */
Camera.prototype.SetTargetBuffers = function(arg0/* RenderBuffer[] */, arg1/* RenderBuffer */) { return CS.Call(4, 1, 42, false, this, arg0,arg1); }
/* Single */
Camera.prototype.get_pixelWidth = function() { return CS.Call(4, 1, 43, false, this); }
/* Single */
Camera.prototype.get_pixelHeight = function() { return CS.Call(4, 1, 44, false, this); }
/* Matrix4x4 */
Camera.prototype.get_cameraToWorldMatrix = function() { return CS.Call(4, 1, 45, false, this); }
/* Matrix4x4 */
Camera.prototype.get_worldToCameraMatrix = function() { return CS.Call(4, 1, 46, false, this); }
/* Void */
Camera.prototype.set_worldToCameraMatrix = function(arg0/* Matrix4x4 */) { return CS.Call(4, 1, 47, false, this, arg0); }
/* Void */
Camera.prototype.ResetWorldToCameraMatrix = function() { return CS.Call(4, 1, 48, false, this); }
/* Matrix4x4 */
Camera.prototype.get_projectionMatrix = function() { return CS.Call(4, 1, 49, false, this); }
/* Void */
Camera.prototype.set_projectionMatrix = function(arg0/* Matrix4x4 */) { return CS.Call(4, 1, 50, false, this, arg0); }
/* Void */
Camera.prototype.ResetProjectionMatrix = function() { return CS.Call(4, 1, 51, false, this); }
/* Void */
Camera.prototype.ResetAspect = function() { return CS.Call(4, 1, 52, false, this); }
/* Vector3 */
Camera.prototype.get_velocity = function() { return CS.Call(4, 1, 53, false, this); }
/* CameraClearFlags */
Camera.prototype.get_clearFlags = function() { return CS.Call(4, 1, 54, false, this); }
/* Void */
Camera.prototype.set_clearFlags = function(arg0/* CameraClearFlags */) { return CS.Call(4, 1, 55, false, this, arg0); }
/* Boolean */
Camera.prototype.get_stereoEnabled = function() { return CS.Call(4, 1, 56, false, this); }
/* Single */
Camera.prototype.get_stereoSeparation = function() { return CS.Call(4, 1, 57, false, this); }
/* Void */
Camera.prototype.set_stereoSeparation = function(arg0/* Single */) { return CS.Call(4, 1, 58, false, this, arg0); }
/* Single */
Camera.prototype.get_stereoConvergence = function() { return CS.Call(4, 1, 59, false, this); }
/* Void */
Camera.prototype.set_stereoConvergence = function(arg0/* Single */) { return CS.Call(4, 1, 60, false, this, arg0); }
/* Vector3 */
Camera.prototype.WorldToScreenPoint = function(arg0/* Vector3 */) { return CS.Call(4, 1, 61, false, this, arg0); }
/* Vector3 */
Camera.prototype.WorldToViewportPoint = function(arg0/* Vector3 */) { return CS.Call(4, 1, 62, false, this, arg0); }
/* Vector3 */
Camera.prototype.ViewportToWorldPoint = function(arg0/* Vector3 */) { return CS.Call(4, 1, 63, false, this, arg0); }
/* Vector3 */
Camera.prototype.ScreenToWorldPoint = function(arg0/* Vector3 */) { return CS.Call(4, 1, 64, false, this, arg0); }
/* Vector3 */
Camera.prototype.ScreenToViewportPoint = function(arg0/* Vector3 */) { return CS.Call(4, 1, 65, false, this, arg0); }
/* Vector3 */
Camera.prototype.ViewportToScreenPoint = function(arg0/* Vector3 */) { return CS.Call(4, 1, 66, false, this, arg0); }
/* Ray */
Camera.prototype.ViewportPointToRay = function(arg0/* Vector3 */) { return CS.Call(4, 1, 67, false, this, arg0); }
/* Ray */
Camera.prototype.ScreenPointToRay = function(arg0/* Vector3 */) { return CS.Call(4, 1, 68, false, this, arg0); }
/* static Camera */
Camera.get_main = function() { return CS.Call(4, 1, 69, true); }
/* static Camera */
Camera.get_current = function() { return CS.Call(4, 1, 70, true); }
/* static Camera[] */
Camera.get_allCameras = function() { return CS.Call(4, 1, 71, true); }
/* static Int32 */
Camera.get_allCamerasCount = function() { return CS.Call(4, 1, 72, true); }
/* static Int32 */
Camera.GetAllCameras = function(arg0/* Camera[] */) { return CS.Call(4, 1, 73, true, arg0); }
/* static Camera */
Camera.get_mainCamera = function() { return CS.Call(4, 1, 74, true); }
/* Single */
Camera.prototype.GetScreenWidth = function() { return CS.Call(4, 1, 75, false, this); }
/* Single */
Camera.prototype.GetScreenHeight = function() { return CS.Call(4, 1, 76, false, this); }
/* Void */
Camera.prototype.DoClear = function() { return CS.Call(4, 1, 77, false, this); }
/* Void */
Camera.prototype.Render = function() { return CS.Call(4, 1, 78, false, this); }
/* Void */
Camera.prototype.RenderWithShader = function(arg0/* Shader */, arg1/* String */) { return CS.Call(4, 1, 79, false, this, arg0,arg1); }
/* Void */
Camera.prototype.SetReplacementShader = function(arg0/* Shader */, arg1/* String */) { return CS.Call(4, 1, 80, false, this, arg0,arg1); }
/* Void */
Camera.prototype.ResetReplacementShader = function() { return CS.Call(4, 1, 81, false, this); }
/* Boolean */
Camera.prototype.get_useOcclusionCulling = function() { return CS.Call(4, 1, 82, false, this); }
/* Void */
Camera.prototype.set_useOcclusionCulling = function(arg0/* Boolean */) { return CS.Call(4, 1, 83, false, this, arg0); }
/* Void */
Camera.prototype.RenderDontRestore = function() { return CS.Call(4, 1, 84, false, this); }
/* static Void */
Camera.SetupCurrent = function(arg0/* Camera */) { return CS.Call(4, 1, 85, true, arg0); }
/* Boolean */
Camera.prototype.RenderToCubemap = function(arg0/* Cubemap */) { return CS.Call(4, 1, 86, false, this, arg0); }
/* Boolean */
Camera.prototype.RenderToCubemap = function(arg0/* Cubemap */, arg1/* Int32 */) { return CS.Call(4, 1, 87, false, this, arg0,arg1); }
/* Boolean */
Camera.prototype.RenderToCubemap = function(arg0/* RenderTexture */) { return CS.Call(4, 1, 88, false, this, arg0); }
/* Boolean */
Camera.prototype.RenderToCubemap = function(arg0/* RenderTexture */, arg1/* Int32 */) { return CS.Call(4, 1, 89, false, this, arg0,arg1); }
/* Single[] */
Camera.prototype.get_layerCullDistances = function() { return CS.Call(4, 1, 90, false, this); }
/* Void */
Camera.prototype.set_layerCullDistances = function(arg0/* Single[] */) { return CS.Call(4, 1, 91, false, this, arg0); }
/* Boolean */
Camera.prototype.get_layerCullSpherical = function() { return CS.Call(4, 1, 92, false, this); }
/* Void */
Camera.prototype.set_layerCullSpherical = function(arg0/* Boolean */) { return CS.Call(4, 1, 93, false, this, arg0); }
/* Void */
Camera.prototype.CopyFrom = function(arg0/* Camera */) { return CS.Call(4, 1, 94, false, this, arg0); }
/* DepthTextureMode */
Camera.prototype.get_depthTextureMode = function() { return CS.Call(4, 1, 95, false, this); }
/* Void */
Camera.prototype.set_depthTextureMode = function(arg0/* DepthTextureMode */) { return CS.Call(4, 1, 96, false, this, arg0); }
/* Boolean */
Camera.prototype.get_clearStencilAfterLightingPass = function() { return CS.Call(4, 1, 97, false, this); }
/* Void */
Camera.prototype.set_clearStencilAfterLightingPass = function(arg0/* Boolean */) { return CS.Call(4, 1, 98, false, this, arg0); }
/* Matrix4x4 */
Camera.prototype.CalculateObliqueMatrix = function(arg0/* Vector4 */) { return CS.Call(4, 1, 99, false, this, arg0); }
/* Boolean */
Camera.prototype.get_enabled = function() { return CS.Call(4, 1, 100, false, this); }
/* Void */
Camera.prototype.set_enabled = function(arg0/* Boolean */) { return CS.Call(4, 1, 101, false, this, arg0); }
/* Transform */
Camera.prototype.get_transform = function() { return CS.Call(4, 1, 102, false, this); }
/* Rigidbody */
Camera.prototype.get_rigidbody = function() { return CS.Call(4, 1, 103, false, this); }
/* Rigidbody2D */
Camera.prototype.get_rigidbody2D = function() { return CS.Call(4, 1, 104, false, this); }
/* Camera */
Camera.prototype.get_camera = function() { return CS.Call(4, 1, 105, false, this); }
/* Light */
Camera.prototype.get_light = function() { return CS.Call(4, 1, 106, false, this); }
/* Animation */
Camera.prototype.get_animation = function() { return CS.Call(4, 1, 107, false, this); }
/* ConstantForce */
Camera.prototype.get_constantForce = function() { return CS.Call(4, 1, 108, false, this); }
/* Renderer */
Camera.prototype.get_renderer = function() { return CS.Call(4, 1, 109, false, this); }
/* AudioSource */
Camera.prototype.get_audio = function() { return CS.Call(4, 1, 110, false, this); }
/* GUIText */
Camera.prototype.get_guiText = function() { return CS.Call(4, 1, 111, false, this); }
/* NetworkView */
Camera.prototype.get_networkView = function() { return CS.Call(4, 1, 112, false, this); }
/* GUIElement */
Camera.prototype.get_guiElement = function() { return CS.Call(4, 1, 113, false, this); }
/* GUITexture */
Camera.prototype.get_guiTexture = function() { return CS.Call(4, 1, 114, false, this); }
/* Collider */
Camera.prototype.get_collider = function() { return CS.Call(4, 1, 115, false, this); }
/* Collider2D */
Camera.prototype.get_collider2D = function() { return CS.Call(4, 1, 116, false, this); }
/* HingeJoint */
Camera.prototype.get_hingeJoint = function() { return CS.Call(4, 1, 117, false, this); }
/* ParticleEmitter */
Camera.prototype.get_particleEmitter = function() { return CS.Call(4, 1, 118, false, this); }
/* ParticleSystem */
Camera.prototype.get_particleSystem = function() { return CS.Call(4, 1, 119, false, this); }
/* GameObject */
Camera.prototype.get_gameObject = function() { return CS.Call(4, 1, 120, false, this); }
/* Component */
Camera.prototype.GetComponent = function(arg0/* Type */) { return CS.Call(4, 1, 121, false, this, arg0); }
/* T */
Camera.prototype.GetComponent = function() { return CS.Call(4, 1, 122, false, this); }
/* Component */
Camera.prototype.GetComponent = function(arg0/* String */) { return CS.Call(4, 1, 123, false, this, arg0); }
/* Component */
Camera.prototype.GetComponentInChildren = function(arg0/* Type */) { return CS.Call(4, 1, 124, false, this, arg0); }
/* T */
Camera.prototype.GetComponentInChildren = function() { return CS.Call(4, 1, 125, false, this); }
/* Component[] */
Camera.prototype.GetComponentsInChildren = function(arg0/* Type */) { return CS.Call(4, 1, 126, false, this, arg0); }
/* Component[] */
Camera.prototype.GetComponentsInChildren = function(arg0/* Type */, arg1/* Boolean */) { return CS.Call(4, 1, 127, false, this, arg0,arg1); }
/* T[] */
Camera.prototype.GetComponentsInChildren = function(arg0/* Boolean */) { return CS.Call(4, 1, 128, false, this, arg0); }
/* T[] */
Camera.prototype.GetComponentsInChildren = function() { return CS.Call(4, 1, 129, false, this); }
/* Component */
Camera.prototype.GetComponentInParent = function(arg0/* Type */) { return CS.Call(4, 1, 130, false, this, arg0); }
/* T */
Camera.prototype.GetComponentInParent = function() { return CS.Call(4, 1, 131, false, this); }
/* Component[] */
Camera.prototype.GetComponentsInParent = function(arg0/* Type */) { return CS.Call(4, 1, 132, false, this, arg0); }
/* Component[] */
Camera.prototype.GetComponentsInParent = function(arg0/* Type */, arg1/* Boolean */) { return CS.Call(4, 1, 133, false, this, arg0,arg1); }
/* T[] */
Camera.prototype.GetComponentsInParent = function(arg0/* Boolean */) { return CS.Call(4, 1, 134, false, this, arg0); }
/* T[] */
Camera.prototype.GetComponentsInParent = function() { return CS.Call(4, 1, 135, false, this); }
/* Component[] */
Camera.prototype.GetComponents = function(arg0/* Type */) { return CS.Call(4, 1, 136, false, this, arg0); }
/* T[] */
Camera.prototype.GetComponents = function() { return CS.Call(4, 1, 137, false, this); }
/* Boolean */
Camera.prototype.get_active = function() { return CS.Call(4, 1, 138, false, this); }
/* Void */
Camera.prototype.set_active = function(arg0/* Boolean */) { return CS.Call(4, 1, 139, false, this, arg0); }
/* String */
Camera.prototype.get_tag = function() { return CS.Call(4, 1, 140, false, this); }
/* Void */
Camera.prototype.set_tag = function(arg0/* String */) { return CS.Call(4, 1, 141, false, this, arg0); }
/* Boolean */
Camera.prototype.CompareTag = function(arg0/* String */) { return CS.Call(4, 1, 142, false, this, arg0); }
/* Void */
Camera.prototype.SendMessageUpwards = function(arg0/* String */, arg1/* Object */, arg2/* SendMessageOptions */) { return CS.Call(4, 1, 143, false, this, arg0,arg1,arg2); }
/* Void */
Camera.prototype.SendMessageUpwards = function(arg0/* String */, arg1/* Object */) { return CS.Call(4, 1, 144, false, this, arg0,arg1); }
/* Void */
Camera.prototype.SendMessageUpwards = function(arg0/* String */) { return CS.Call(4, 1, 145, false, this, arg0); }
/* Void */
Camera.prototype.SendMessageUpwards = function(arg0/* String */, arg1/* SendMessageOptions */) { return CS.Call(4, 1, 146, false, this, arg0,arg1); }
/* Void */
Camera.prototype.SendMessage = function(arg0/* String */, arg1/* Object */, arg2/* SendMessageOptions */) { return CS.Call(4, 1, 147, false, this, arg0,arg1,arg2); }
/* Void */
Camera.prototype.SendMessage = function(arg0/* String */, arg1/* Object */) { return CS.Call(4, 1, 148, false, this, arg0,arg1); }
/* Void */
Camera.prototype.SendMessage = function(arg0/* String */) { return CS.Call(4, 1, 149, false, this, arg0); }
/* Void */
Camera.prototype.SendMessage = function(arg0/* String */, arg1/* SendMessageOptions */) { return CS.Call(4, 1, 150, false, this, arg0,arg1); }
/* Void */
Camera.prototype.BroadcastMessage = function(arg0/* String */, arg1/* Object */, arg2/* SendMessageOptions */) { return CS.Call(4, 1, 151, false, this, arg0,arg1,arg2); }
/* Void */
Camera.prototype.BroadcastMessage = function(arg0/* String */, arg1/* Object */) { return CS.Call(4, 1, 152, false, this, arg0,arg1); }
/* Void */
Camera.prototype.BroadcastMessage = function(arg0/* String */) { return CS.Call(4, 1, 153, false, this, arg0); }
/* Void */
Camera.prototype.BroadcastMessage = function(arg0/* String */, arg1/* SendMessageOptions */) { return CS.Call(4, 1, 154, false, this, arg0,arg1); }
/* Boolean */
Camera.prototype.Equals = function(arg0/* Object */) { return CS.Call(4, 1, 155, false, this, arg0); }
/* Int32 */
Camera.prototype.GetHashCode = function() { return CS.Call(4, 1, 156, false, this); }
/* Int32 */
Camera.prototype.GetInstanceID = function() { return CS.Call(4, 1, 157, false, this); }
/* String */
Camera.prototype.get_name = function() { return CS.Call(4, 1, 158, false, this); }
/* Void */
Camera.prototype.set_name = function(arg0/* String */) { return CS.Call(4, 1, 159, false, this, arg0); }
/* HideFlags */
Camera.prototype.get_hideFlags = function() { return CS.Call(4, 1, 160, false, this); }
/* Void */
Camera.prototype.set_hideFlags = function(arg0/* HideFlags */) { return CS.Call(4, 1, 161, false, this, arg0); }
/* String */
Camera.prototype.ToString = function() { return CS.Call(4, 1, 162, false, this); }
/* Type */
Camera.prototype.GetType = function() { return CS.Call(4, 1, 163, false, this); }
