namespace UPDB.CoreHelper
{
    public class NamespaceID
    {
        public const string UPDB = nameof(UPDB);

        public const string Ai = nameof(Ai);
        public const string CamerasAndCharacterControllers = nameof(CamerasAndCharacterControllers);
        public const string CoreHelper = nameof(CoreHelper);
        public const string CustomInput = nameof(CustomInput);
        public const string Data = nameof(Data);
        public const string Physic = nameof(Physic);
        public const string ProceduralGeneration = nameof(ProceduralGeneration);
        public const string Shaders = nameof(Shaders);
        public const string Sound = nameof(Sound);

        public const string StateMachineTool = nameof(StateMachineTool);
        public const string Cameras = nameof(Cameras);
        public const string CharacterControllers = nameof(CharacterControllers);
        public const string CustomPropertyAttributes = nameof(CustomPropertyAttributes);
        public const string CustomScriptOrderMethods = nameof(CustomScriptOrderMethods);
        public const string UsableMethods = nameof(UsableMethods);
        public const string Usable = nameof(Usable);
        public const string RotationDetector = nameof(RotationDetector);
        public const string NativeTools = nameof(NativeTools);
        public const string SaveStateManager = nameof(SaveStateManager);
        public const string SimpleCraft = nameof(SimpleCraft);
        public const string SplineTool = nameof(SplineTool);
        public const string UITools = nameof(UITools);
        public const string AutoRotate = nameof(AutoRotate);
        public const string CustomTimeScale = nameof(CustomTimeScale);
        public const string Grabber = nameof(Grabber);
        public const string GravityManager = nameof(GravityManager);
        public const string RAPhysic = nameof(RAPhysic);
        public const string SMPhysic = nameof(SMPhysic);
        public const string CartoonWind = nameof(CartoonWind);
        public const string MazeGenerator = nameof(MazeGenerator);
        public const string NoiseGenerator = nameof(NoiseGenerator);
        public const string ProShapeBuilder = nameof(ProShapeBuilder);
        public const string ProTerrainBuilder = nameof(ProTerrainBuilder);
        public const string TextureNanite = nameof(TextureNanite);
        public const string AmbianceMixer = nameof(AmbianceMixer);
        public const string EngineGear = nameof(EngineGear);

        public const string SimpleFpsCamera = nameof(SimpleFpsCamera);
        public const string SimpleFreeCamera = nameof(SimpleFreeCamera);
        public const string SimpleGenericCamera = nameof(SimpleGenericCamera);
        public const string Smooth25DCameraController = nameof(Smooth25DCameraController);
        public const string TpsCamera = nameof(TpsCamera);
        public const string CompleteTpsController = nameof(CompleteTpsController);
        public const string ProceduralTpsController = nameof(ProceduralTpsController);
        public const string RbFpsController = nameof(RbFpsController);
        public const string SpriteTpsController = nameof(SpriteTpsController);
        public const string Structures = nameof(Structures);
        public const string SimpleGridLevel = nameof(SimpleGridLevel);
        public const string MenuUIController = nameof(MenuUIController);
        public const string ClassicMazeGenerator = nameof(ClassicMazeGenerator);

        /*******************************COMBINATION OF PATHS*********************************/

        //CamerasAndCharacterControllers
        public const string CamerasAndCharacterControllersPath = UPDB + "/" + CamerasAndCharacterControllers;
        public const string CamerasPath = CamerasAndCharacterControllersPath + "/" + Cameras;
        public const string CharacterControllersPath = CamerasAndCharacterControllersPath + "/" + CharacterControllers;

        //CoreHelper
        public const string CoreHelperPath = UPDB + "/" + CoreHelper;
        public const string CustomPropertyAttributesPath = CoreHelperPath + "/" + CustomPropertyAttributes;
        public const string CustomScriptOrderMethodsPath = CoreHelperPath + "/" + CustomScriptOrderMethods;
        public const string UsableMethodsPath = CoreHelperPath + "/" + UsableMethods;
        public const string UsablePath = CoreHelperPath + "/" + Usable;

        //CustomInput
        public const string CustomInputPath = UPDB + "/" + CustomInput;


        //Data
        public const string DataPath = UPDB + "/" + Data;


        //Physic
        public const string PhysicPath = UPDB + "/" + Physic;


        //ProceduralGeneration
        public const string ProceduralGenerationPath = UPDB + "/" + ProceduralGeneration;


        //Shaders
        public const string ShadersPath = UPDB + "/" + Shaders;


        //Sound
        public const string SoundPath = UPDB + "/" + Sound;
    }
}
