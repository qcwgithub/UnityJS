using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

public class JSBindingSettings
{
    public static Type[] enums = new Type[]
    {
	    typeof(UnityEngine.FilterMode),
        typeof(UnityEngine.TextureWrapMode),
        typeof(UnityEngine.NPOTSupport),
        typeof(UnityEngine.TextureFormat),
        typeof(UnityEngine.CubemapFace),
        typeof(UnityEngine.RenderTextureFormat),
        typeof(UnityEngine.RenderTextureReadWrite),
        typeof(UnityEngine.Rendering.BlendMode),
        typeof(UnityEngine.Rendering.BlendOp),
        typeof(UnityEngine.Rendering.CompareFunction),
        typeof(UnityEngine.Rendering.CullMode),
        typeof(UnityEngine.Rendering.ColorWriteMask),
        typeof(UnityEngine.Rendering.StencilOp),
        typeof(UnityEngine.SocialPlatforms.UserState),
        typeof(UnityEngine.SocialPlatforms.UserScope),
        typeof(UnityEngine.SocialPlatforms.TimeScope),
        typeof(UnityEngineInternal.TypeInferenceRules),
        typeof(UnityEngine.ForceMode),
        typeof(UnityEngine.RigidbodyConstraints),
        typeof(UnityEngine.RigidbodyInterpolation),
        typeof(UnityEngine.JointDriveMode),
        typeof(UnityEngine.JointProjectionMode),
        typeof(UnityEngine.ConfigurableJointMotion),
        typeof(UnityEngine.RotationDriveMode),
        typeof(UnityEngine.CollisionDetectionMode),
        typeof(UnityEngine.PhysicMaterialCombine),
        typeof(UnityEngine.CollisionFlags),
        typeof(UnityEngine.RigidbodyInterpolation2D),
        typeof(UnityEngine.RigidbodySleepMode2D),
        typeof(UnityEngine.CollisionDetectionMode2D),
        typeof(UnityEngine.ForceMode2D),
        typeof(UnityEngine.JointLimitState2D),
        typeof(UnityEngine.ObstacleAvoidanceType),
        typeof(UnityEngine.OffMeshLinkType),
        typeof(UnityEngine.NavMeshPathStatus),
        typeof(UnityEngine.AudioSpeakerMode),
        typeof(UnityEngine.AudioType),
        typeof(UnityEngine.AudioVelocityUpdateMode),
        typeof(UnityEngine.FFTWindow),
        typeof(UnityEngine.AudioRolloffMode),
        typeof(UnityEngine.AudioReverbPreset),
        typeof(UnityEngine.WebCamFlags),
        typeof(UnityEngine.WrapMode),
        typeof(UnityEngine.PlayMode),
        typeof(UnityEngine.QueueMode),
        typeof(UnityEngine.AnimationBlendMode),
        typeof(UnityEngine.AnimationPlayMode),
        typeof(UnityEngine.AnimationCullingType),
        typeof(UnityEngine.AvatarTarget),
        typeof(UnityEngine.AvatarIKGoal),
        typeof(UnityEngine.AnimatorCullingMode),
        typeof(UnityEngine.AnimatorUpdateMode),
        typeof(UnityEngine.HumanBodyBones),
        typeof(UnityEngine.DetailRenderMode),
        typeof(UnityEngine.TerrainRenderFlags),
        typeof(UnityEngine.HideFlags),
        typeof(UnityEngine.SendMessageOptions),
        typeof(UnityEngine.PrimitiveType),
        typeof(UnityEngine.Space),
        typeof(UnityEngine.RuntimePlatform),
        typeof(UnityEngine.SystemLanguage),
        typeof(UnityEngine.LogType),
        typeof(UnityEngine.DeviceType),
        typeof(UnityEngine.ThreadPriority),
        typeof(UnityEngine.CursorMode),
        typeof(UnityEngine.LightType),
        typeof(UnityEngine.LightRenderMode),
        typeof(UnityEngine.LightShadows),
        typeof(UnityEngine.FogMode),
        typeof(UnityEngine.QualityLevel),
        typeof(UnityEngine.ShadowProjection),
        typeof(UnityEngine.CameraClearFlags),
        typeof(UnityEngine.DepthTextureMode),
        typeof(UnityEngine.TexGenMode),
        typeof(UnityEngine.AnisotropicFiltering),
        typeof(UnityEngine.BlendWeights),
        typeof(UnityEngine.TextureCompressionQuality),
        typeof(UnityEngine.MeshTopology),
        typeof(UnityEngine.SkinQuality),
        typeof(UnityEngine.ParticleRenderMode),
        typeof(UnityEngine.LightmapsMode),
        typeof(UnityEngine.ColorSpace),
        typeof(UnityEngine.ScreenOrientation),
        typeof(UnityEngine.TextAlignment),
        typeof(UnityEngine.TextAnchor),
        typeof(UnityEngine.ScaleMode),
        typeof(UnityEngine.FocusType),
        typeof(UnityEngine.FontStyle),
        typeof(UnityEngine.TextWrapMode),
        typeof(UnityEngine.ImagePosition),
        typeof(UnityEngine.TextClipping),
        typeof(UnityEngine.FullScreenMovieControlMode),
        typeof(UnityEngine.FullScreenMovieScalingMode),
        typeof(UnityEngine.iOSActivityIndicatorStyle),
        typeof(UnityEngine.AndroidActivityIndicatorStyle),
        typeof(UnityEngine.TouchScreenKeyboardType),
        typeof(UnityEngine.iPhoneGeneration),
        typeof(UnityEngine.KeyCode),
        typeof(UnityEngine.EventType),
        typeof(UnityEngine.EventModifiers),
        typeof(UnityEngine.iPhoneTouchPhase),
        typeof(UnityEngine.iPhoneOrientation),
        typeof(UnityEngine.iPhoneScreenOrientation),
        typeof(UnityEngine.iPhoneKeyboardType),
        typeof(UnityEngine.iPhoneMovieControlMode),
        typeof(UnityEngine.iPhoneMovieScalingMode),
        typeof(UnityEngine.iPhoneNetworkReachability),
        typeof(UnityEngine.CalendarIdentifier),
        typeof(UnityEngine.CalendarUnit),
        typeof(UnityEngine.RemoteNotificationType),
        typeof(UnityEngine.RPCMode),
        typeof(UnityEngine.ConnectionTesterStatus),
        typeof(UnityEngine.NetworkConnectionError),
        typeof(UnityEngine.NetworkDisconnection),
        typeof(UnityEngine.MasterServerEvent),
        typeof(UnityEngine.NetworkStateSynchronization),
        typeof(UnityEngine.NetworkPeerType),
        typeof(UnityEngine.NetworkLogLevel),
        typeof(UnityEngine.ParticleSystemRenderMode),
        typeof(UnityEngine.ParticleSystemSimulationSpace),
        typeof(UnityEngine.ProceduralProcessorUsage),
        typeof(UnityEngine.ProceduralCacheSize),
        typeof(UnityEngine.ProceduralLoadingBehavior),
        typeof(UnityEngine.ProceduralPropertyType),
        typeof(UnityEngine.ProceduralOutputType),
        typeof(UnityEngine.SpriteAlignment),
        typeof(UnityEngine.SpritePackingMode),
        typeof(UnityEngine.SpritePackingRotation),
        typeof(UnityEngine.SpriteMeshType),
        typeof(UnityEngine.NetworkReachability),
        typeof(UnityEngine.UserAuthorization),
        typeof(UnityEngine.RenderingPath),
        typeof(UnityEngine.TransparencySortMode),
        typeof(UnityEngine.ComputeBufferType),
        typeof(UnityEngine.TouchPhase),
        typeof(UnityEngine.IMECompositionMode),
        typeof(UnityEngine.DeviceOrientation),
        typeof(UnityEngine.LocationServiceStatus),
    };

    public static Type[] classes = new Type[]
    {
        // interface

        //typeof(UnityEngine.SocialPlatforms.ISocialPlatform), //x
//         typeof(UnityEngine.SocialPlatforms.ILocalUser),//ok
//         typeof(UnityEngine.SocialPlatforms.IUserProfile),//ok
//         typeof(UnityEngine.SocialPlatforms.IAchievement),//ok
//         typeof(UnityEngine.SocialPlatforms.IAchievementDescription),//ok
//         typeof(UnityEngine.SocialPlatforms.IScore),//ok
//         typeof(UnityEngine.SocialPlatforms.ILeaderboard),//ok
//         typeof(UnityEngine.ISerializationCallbackReceiver),//ok

        // class

//         typeof(UnityEngine.AndroidJavaException), //x
//         typeof(UnityEngine.AndroidJavaProxy),
//         typeof(UnityEngine.AndroidJavaObject),
//         typeof(UnityEngine.AndroidJavaClass),
//         typeof(AOT.MonoPInvokeCallbackAttribute),
//         typeof(UnityEngine.ThreadSafeAttribute),

//         typeof(UnityEngine.ConstructorSafeAttribute),
//         typeof(UnityEngine.AssemblyIsEditorAssembly),
//         typeof(UnityEngine.SocialPlatforms.GameCenter.GameCenterPlatform), //X
//         typeof(UnityEngine.ImplementedInActionScriptAttribute),
//         typeof(UnityEngine.SocialPlatforms.Impl.LocalUser),
//         typeof(UnityEngine.SocialPlatforms.Impl.UserProfile),
//         typeof(UnityEngine.SocialPlatforms.Impl.Achievement),
//         typeof(UnityEngine.SocialPlatforms.Impl.AchievementDescription),
//         typeof(UnityEngine.SocialPlatforms.Impl.Score),
//         typeof(UnityEngine.SocialPlatforms.Impl.Leaderboard),
//         typeof(UnityEngine.SocialPlatforms.Local),

/*
        typeof(UnityEngine.Social),
        typeof(UnityEngine.PropertyAttribute),
        typeof(UnityEngine.ContextMenuItemAttribute),
        typeof(UnityEngine.TooltipAttribute),
        typeof(UnityEngine.SpaceAttribute),
        typeof(UnityEngine.HeaderAttribute),
        typeof(UnityEngine.RangeAttribute),
        typeof(UnityEngine.MultilineAttribute),
        typeof(UnityEngine.TextAreaAttribute),
        typeof(UnityEngine.Security),
        //typeof(UnityEngine.Types), // GetType 可能与父类重复

        typeof(UnityEngine.SelectionBaseAttribute),
        typeof(UnityEngine.StackTraceUtility),

        //typeof(UnityEngine.UnityException),
        // typeof(UnityEngine.MissingComponentException), // GetType 可能与父类重复
        //typeof(UnityEngine.UnassignedReferenceException), // GetType 可能与父类重复
        // typeof(UnityEngine.MissingReferenceException), // GetType 可能与父类重复
        //typeof(UnityEngine.TextEditor),//x 有+

        typeof(UnityEngine.TextGenerator),
        typeof(UnityEngine.TrackedReference),
        typeof(UnityEngine.WWW),*/


//         typeof(UnityEngine.Internal.DefaultValueAttribute),
//         typeof(UnityEngine.Internal.ExcludeFromDocsAttribute),
//         typeof(UnityEngine.iPhone),
//         typeof(UnityEngine.ADBannerView),
//         typeof(UnityEngine.ADInterstitialAd),
//         typeof(UnityEngine.Serialization.UnitySurrogateSelector),
//         typeof(UnityEngineInternal.TypeInferenceRuleAttribute),
//         typeof(UnityEngineInternal.GenericStack),
//         typeof(UnityEngine.Physics),
//         typeof(UnityEngine.Rigidbody),
//         typeof(UnityEngine.Joint),
//         typeof(UnityEngine.HingeJoint),//ok
//         typeof(UnityEngine.SpringJoint),//ok
//         typeof(UnityEngine.FixedJoint),//ok
//         typeof(UnityEngine.CharacterJoint),//ok
//         typeof(UnityEngine.ConfigurableJoint),//ok
//         typeof(UnityEngine.ConstantForce),//ok
//         typeof(UnityEngine.Collider),//ok
//         typeof(UnityEngine.BoxCollider),//ok
//         typeof(UnityEngine.SphereCollider),//ok
//         typeof(UnityEngine.MeshCollider),//ok
//         typeof(UnityEngine.CapsuleCollider),//ok
//         typeof(UnityEngine.RaycastCollider),//ok
//         typeof(UnityEngine.WheelCollider),//ok
//         typeof(UnityEngine.PhysicMaterial),//ok
//         typeof(UnityEngine.Collision),//ok
//         typeof(UnityEngine.ControllerColliderHit),//ok
//         typeof(UnityEngine.CharacterController),//ok
//         typeof(UnityEngine.Cloth),//ok
//         typeof(UnityEngine.InteractiveCloth),//ok
//         typeof(UnityEngine.SkinnedCloth),//ok
//         typeof(UnityEngine.ClothRenderer),//ok
//         typeof(UnityEngine.TerrainCollider),//ok
//         typeof(UnityEngine.Physics2D),//ok
//         typeof(UnityEngine.Rigidbody2D),//ok
//         typeof(UnityEngine.Collider2D),//ok
//         typeof(UnityEngine.CircleCollider2D),//ok
//         typeof(UnityEngine.BoxCollider2D),//ok
//         typeof(UnityEngine.EdgeCollider2D),//ok
//         typeof(UnityEngine.PolygonCollider2D),//ok
//         typeof(UnityEngine.Collision2D),//ok
//         typeof(UnityEngine.Joint2D),//ok
//         typeof(UnityEngine.AnchoredJoint2D),//ok
//         typeof(UnityEngine.SpringJoint2D),//ok
//         typeof(UnityEngine.DistanceJoint2D),//ok
//         typeof(UnityEngine.HingeJoint2D),//ok

//         typeof(UnityEngine.SliderJoint2D),//ok
//         typeof(UnityEngine.WheelJoint2D),//ok
//         typeof(UnityEngine.PhysicsMaterial2D),//ok
//         typeof(UnityEngine.NavMeshAgent),//ok
//         typeof(UnityEngine.NavMesh),//ok
//         typeof(UnityEngine.OffMeshLink),//ok
//         typeof(UnityEngine.NavMeshPath),//ok
//         typeof(UnityEngine.NavMeshObstacle),//ok
//         typeof(UnityEngine.AudioSettings),//ok

//         typeof(UnityEngine.AudioClip),//ok
//         typeof(UnityEngine.AudioListener),//ok
        //typeof(UnityEngine.AudioSource),//Obsolete问题！
//         typeof(UnityEngine.AudioReverbZone),//ok
//         typeof(UnityEngine.AudioLowPassFilter),//ok

//         typeof(UnityEngine.AudioHighPassFilter),//ok
//         typeof(UnityEngine.AudioDistortionFilter),//ok
//         typeof(UnityEngine.AudioEchoFilter),//ok
//         typeof(UnityEngine.AudioChorusFilter),//ok
//         typeof(UnityEngine.AudioReverbFilter),//ok
//         typeof(UnityEngine.Microphone),//ok
//         typeof(UnityEngine.MovieTexture),//ok
//         typeof(UnityEngine.WebCamTexture),//ok
//         typeof(UnityEngine.AnimationClipPair),//ok
        //typeof(UnityEngine.AnimatorOverrideController),//重载函数排序问题
//          typeof(UnityEngine.AnimationEvent),//ok
//          typeof(UnityEngine.AnimationClip),//ok
// typeof(UnityEngine.AnimationCurve),////ok
        //typeof(UnityEngine.Animation),//重载函数排序问题
//         typeof(UnityEngine.AnimationState),//ok
         //typeof(UnityEngine.GameObject),//ok
//         typeof(UnityEngine.Animator),//ok
//         typeof(UnityEngine.AvatarBuilder),//ok
//         typeof(UnityEngine.RuntimeAnimatorController),//ok
//         typeof(UnityEngine.Avatar),//ok
//         typeof(UnityEngine.HumanTrait),//ok
//         typeof(UnityEngine.TreePrototype),//ok
//         typeof(UnityEngine.DetailPrototype),//ok
//         typeof(UnityEngine.SplatPrototype),//ok
//         typeof(UnityEngine.TerrainData),//ok
//          typeof(UnityEngine.Terrain),//ok
//         typeof(UnityEngine.Tree),                                              //ok
//         typeof(UnityEngine.AssetBundleCreateRequest),                          //ok
//         typeof(UnityEngine.AssetBundleRequest),                                //ok
//         typeof(UnityEngine.AssetBundle),                                       //ok
//         typeof(UnityEngine.SystemInfo),                                        //ok
//         typeof(UnityEngine.WaitForSeconds),                                    //ok
//         typeof(UnityEngine.WaitForFixedUpdate),                                //ok
//         typeof(UnityEngine.WaitForEndOfFrame),                                 //ok
//         typeof(UnityEngine.Coroutine),                                         //ok
//         typeof(UnityEngine.DisallowMultipleComponent),                         //ok
//         typeof(UnityEngine.RequireComponent),                                  //ok
//         typeof(UnityEngine.AddComponentMenu),                                  //ok
//         typeof(UnityEngine.ContextMenu),                                       //ok
//         typeof(UnityEngine.ExecuteInEditMode),                                 //ok
//         typeof(UnityEngine.HideInInspector),                                   //ok
//         typeof(UnityEngine.ScriptableObject),                                  //ok
//         typeof(UnityEngine.Resources),                                         //ok
//         typeof(UnityEngine.Profiler),                                          //ok
//         typeof(UnityEngineInternal.Reproduction),                              //ok
//         typeof(UnityEngine.CrashReport),                                       //ok
//         typeof(UnityEngine.Cursor),                                            //ok
//         typeof(UnityEngine.OcclusionArea),                                     //ok
//         typeof(UnityEngine.OcclusionPortal),                                   //ok
//         typeof(UnityEngine.RenderSettings),                                    //ok
//         typeof(UnityEngine.QualitySettings),                                   //ok
//         typeof(UnityEngine.MeshFilter),                                        //ok
//         typeof(UnityEngine.Mesh),                                              //ok
//         typeof(UnityEngine.SkinnedMeshRenderer),                               //ok
//         typeof(UnityEngine.Flare),                                             //ok
//         typeof(UnityEngine.LensFlare),                                         //ok
//         typeof(UnityEngine.Renderer),                                          //ok
//         typeof(UnityEngine.Projector),                                         //ok
//         typeof(UnityEngine.Skybox),                                            //ok
//         typeof(UnityEngine.TextMesh),                                          //ok
//         typeof(UnityEngine.ParticleEmitter),                                   //ok
//         typeof(UnityEngine.ParticleAnimator),                                  //ok
//         typeof(UnityEngine.TrailRenderer),                                     //ok
//         typeof(UnityEngine.ParticleRenderer),                                  //ok
//         typeof(UnityEngine.LineRenderer),                                      //ok
//         typeof(UnityEngine.MaterialPropertyBlock),                             //ok
//         typeof(UnityEngine.Graphics),                                          //ok
//         typeof(UnityEngine.LightmapData),                                      //ok
//         typeof(UnityEngine.LightProbes),                                       //ok
//         typeof(UnityEngine.LightmapSettings),                                  //ok
//         typeof(UnityEngine.GeometryUtility),                                   //ok
//         typeof(UnityEngine.Screen),                                            //ok
//         typeof(UnityEngine.SleepTimeout),                                      //ok
//         typeof(UnityEngine.GL),                                                //ok
//         typeof(UnityEngine.MeshRenderer),                                      //ok
//         typeof(UnityEngine.StaticBatchingUtility),                             //ok
//         typeof(UnityEngine.ImageEffectTransformsToLDR),                        //ok
//         typeof(UnityEngine.ImageEffectOpaque),                                 //ok
//         typeof(UnityEngine.Texture),                                           //ok
//         typeof(UnityEngine.Texture2D),                                         //ok
//         typeof(UnityEngine.Cubemap),                                           //ok
//         typeof(UnityEngine.Texture3D),                                         //ok
//         typeof(UnityEngine.SparseTexture),                                     //ok
//         typeof(UnityEngine.RenderTexture),                                     //ok
//         typeof(UnityEngine.GUIElement),                                        //ok
//         typeof(UnityEngine.GUITexture),                                        //ok
//         typeof(UnityEngine.GUIText),                                           //ok
//         typeof(UnityEngine.Font),                                              //ok
//         typeof(UnityEngine.GUILayer),                                          //ok
//         typeof(UnityEngine.LODGroup),                                          //ok
//         typeof(UnityEngine.Gradient),                                          //ok
//         typeof(UnityEngine.GUI),
//         typeof(UnityEngine.GUILayout),
//         typeof(UnityEngine.GUILayoutUtility),                                    //ok
//         typeof(UnityEngine.GUILayoutOption),                                     //ok
//        typeof(UnityEngine.ExitGUIException),//GetType问题
//         typeof(UnityEngine.GUIUtility),//ok
//         typeof(UnityEngine.GUISettings),//ok
//         typeof(UnityEngine.GUISkin),//ok
//         typeof(UnityEngine.GUIContent),//ok
//         typeof(UnityEngine.GUIStyleState),//ok
//         typeof(UnityEngine.RectOffset),//ok
//        typeof(UnityEngine.GUIStyle),//!
//         typeof(UnityEngine.Handheld),                                //ok
//         typeof(UnityEngine.TouchScreenKeyboard),                     //ok
//         typeof(UnityEngine.Event),                                   //ok
//         typeof(UnityEngine.Gizmos),                                  //ok
//         typeof(UnityEngine.iPhoneInput),                             //ok
//         typeof(UnityEngine.iPhoneSettings),                          //ok
//         typeof(UnityEngine.iPhoneKeyboard),                          //ok
//         typeof(UnityEngine.iPhoneUtils),                             //ok
//         typeof(UnityEngine.LocalNotification),                       //ok
//         typeof(UnityEngine.RemoteNotification),                      //ok
//         typeof(UnityEngine.NotificationServices),                    //ok
//         typeof(UnityEngine.LightProbeGroup),                         //ok
//         typeof(UnityEngine.Ping),                                    //ok
//         typeof(UnityEngine.NetworkView),                             //ok
//         typeof(UnityEngine.Network),                                 //ok
//         typeof(UnityEngine.BitStream),                               //ok
//         typeof(UnityEngine.RPC),                                     //ok
//         typeof(UnityEngine.HostData),                                //ok
//         typeof(UnityEngine.MasterServer),                            //ok
//         typeof(UnityEngine.ParticleSystem),                          //ok
//         typeof(UnityEngine.ParticleSystemRenderer),                  //ok
//         typeof(UnityEngine.TextAsset),                               //ok
//         typeof(UnityEngine.SerializePrivateVariables),               //ok
//         typeof(UnityEngine.SerializeField),                          //ok
//         typeof(UnityEngine.Shader),                                  //ok
//         typeof(UnityEngine.Material),                                //ok
//         typeof(UnityEngine.ProceduralPropertyDescription),           //ok
//         typeof(UnityEngine.ProceduralMaterial),                      //ok
//         typeof(UnityEngine.ProceduralTexture),                       //ok
//         typeof(UnityEngine.Sprite),                                  //ok
//         typeof(UnityEngine.SpriteRenderer),                          //ok
//         typeof(UnityEngine.Sprites.DataUtility),                     //ok
//         typeof(UnityEngine.WWWForm),                                 //ok
//         typeof(UnityEngine.Caching),                                 //ok
//         typeof(UnityEngine.AsyncOperation),                          //ok
//         typeof(UnityEngine.Application),                             //ok
//         typeof(UnityEngine.Behaviour),                               //ok
//         typeof(UnityEngine.Camera),                                  //ok
//         typeof(UnityEngine.ComputeShader),                           //ok
//         typeof(UnityEngine.ComputeBuffer),                           //ok
//         typeof(UnityEngine.Debug),                                   //ok
//        typeof(UnityEngine.Display),//EVENT!!新问题
//         typeof(UnityEngine.Flash.ActionScript),                      //ok
//         typeof(UnityEngine.Flash.FlashPlayer),                       //ok
//         typeof(UnityEngine.NotConvertedAttribute),                   //ok
//         typeof(UnityEngine.NotFlashValidatedAttribute),              //ok
//         typeof(UnityEngine.NotRenamedAttribute),                     //ok
//         typeof(UnityEngine.MonoBehaviour),                           //ok
//         typeof(UnityEngine.Gyroscope),                               //ok
//         typeof(UnityEngine.LocationService),                         //ok
//         typeof(UnityEngine.Compass),                                 //ok
//         typeof(UnityEngine.Input),                                   //ok
        //typeof(UnityEngine.Object),//不知道！
//         typeof(UnityEngine.Component),                              //ok
//         typeof(UnityEngine.Light),                                  //ok
//         typeof(UnityEngine.Transform),                              //ok
//         typeof(UnityEngine.Time),                                   //ok
        //typeof(UnityEngine.Random),//不知道
//        typeof(UnityEngine.YieldInstruction),
        //typeof(UnityEngine.PlayerPrefsException),//GetType问题
//         typeof(UnityEngine.PlayerPrefs),                              //ok
//         typeof(UnityEngine.AndroidJNIHelper),                         //ok
//         typeof(UnityEngine.AndroidJNI),                               //ok
//         typeof(UnityEngine.AndroidInput),                             //ok
//         typeof(UnityEngine.Motion),                                   //ok
//         typeof(UnityEngine.SamsungTV),                                //ok
//         typeof(UnityEngine.AndroidJavaRunnable),                      //ok
//         typeof(UnityEngine.Events.UnityAction),                       //ok
//         typeof(UnityEngineInternal.FastCallExceptionHandler),         //ok
//         typeof(UnityEngineInternal.GetMethodDelegate),                //ok

        // ValueType

//         typeof(UnityEngine.SocialPlatforms.Range),             //ok
//         typeof(UnityEngine.TextGenerationSettings),            //ok
//         typeof(UnityEngine.JointMotor),                        //ok
//         typeof(UnityEngine.JointSpring),                       //ok
//         typeof(UnityEngine.JointLimits),                       //ok
//         typeof(UnityEngine.SoftJointLimit),                    //ok
//         typeof(UnityEngine.JointDrive),                        //ok
//         typeof(UnityEngine.WheelFrictionCurve),                //ok
//         typeof(UnityEngine.WheelHit),                          //ok
//         typeof(UnityEngine.RaycastHit),                        //ok
//         typeof(UnityEngine.ContactPoint),                      //ok
//         typeof(UnityEngine.ClothSkinningCoefficient),          //ok
//         typeof(UnityEngine.RaycastHit2D),                      //ok
//         typeof(UnityEngine.ContactPoint2D),                    //ok
//         typeof(UnityEngine.JointAngleLimits2D),                //ok
//         typeof(UnityEngine.JointTranslationLimits2D),          //ok
//         typeof(UnityEngine.JointMotor2D),                      //ok
//         typeof(UnityEngine.JointSuspension2D),                 //ok
//         typeof(UnityEngine.OffMeshLinkData),                //ok
//         typeof(UnityEngine.NavMeshHit),                     //ok
//         typeof(UnityEngine.NavMeshTriangulation),           //ok
//         typeof(UnityEngine.WebCamDevice),                   //ok
//         typeof(UnityEngine.Keyframe),                       //ok
//         typeof(UnityEngine.AnimationInfo),                  //ok
//         typeof(UnityEngine.AnimatorStateInfo),              //ok
//         typeof(UnityEngine.AnimatorTransitionInfo),         //ok
//         typeof(UnityEngine.MatchTargetWeightMask),          //ok
//         typeof(UnityEngine.SkeletonBone),                   //ok
//         typeof(UnityEngine.HumanLimit),                     //ok
//         typeof(UnityEngine.HumanBone),                      //ok
//         typeof(UnityEngine.HumanDescription),               //ok
//         typeof(UnityEngine.TreeInstance),                   //ok
//         typeof(UnityEngine.UIVertex),                       //ok
//         typeof(UnityEngine.LayerMask),                      //ok
//         typeof(UnityEngine.CombineInstance),                //ok
//         typeof(UnityEngine.BoneWeight),                     //ok
//         typeof(UnityEngine.Particle),                       //ok
//         typeof(UnityEngine.RenderBuffer),                   //ok
//         typeof(UnityEngine.Resolution),                     //ok
//         typeof(UnityEngine.CharacterInfo),                  //ok
//         typeof(UnityEngine.UICharInfo),                     //ok
//         typeof(UnityEngine.UILineInfo),                     //ok
//         typeof(UnityEngine.LOD),                            //ok
//         typeof(UnityEngine.GradientColorKey),               //ok
//         typeof(UnityEngine.GradientAlphaKey),               //ok
//         typeof(UnityEngine.iPhoneTouch),                    //ok
//         typeof(UnityEngine.iPhoneAccelerationEvent),        //ok
//         typeof(UnityEngine.Vector2),                        //ok
          typeof(UnityEngine.Vector3),                    //ok
//         typeof(UnityEngine.Color),                       //ok
//         typeof(UnityEngine.Color32),                     //ok
//         typeof(UnityEngine.Quaternion),                  //ok
//         typeof(UnityEngine.Rect),                        //ok
//         typeof(UnityEngine.Matrix4x4),                   //ok
//         typeof(UnityEngine.Bounds),                      //ok
//         typeof(UnityEngine.Vector4),                     //ok
//         typeof(UnityEngine.Ray),                         //ok
//         typeof(UnityEngine.Ray2D),                       //ok
//         typeof(UnityEngine.Plane),                       //ok
//         typeof(UnityEngine.Mathf),                       //ok
//         typeof(UnityEngine.NetworkPlayer),               //ok
//         typeof(UnityEngine.NetworkViewID),               //ok
//         typeof(UnityEngine.NetworkMessageInfo),          //ok
//         typeof(UnityEngine.CacheIndex),                  //ok
//         typeof(UnityEngine.Touch),                       //ok
//         typeof(UnityEngine.AccelerationEvent),           //ok
//         typeof(UnityEngine.LocationInfo),                //ok
//         typeof(UnityEngine.jvalue),

    };
}
