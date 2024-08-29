access all tags in project : UnityEditorInternal.InternalEditorUtility.tags
check null value of a native class instance : ReferenceEquals(Class instance, null)    PS : ReferenceEquals(A, B) test a real reference test 100% solid between A and B
make an enum flag : to make a flag of selectable like a layerMask, put the enum definition with [System.Flags] and define numbers to each values spaced enough to not make glitches
ENUM FLAG : by default, test if a specific state is enabled, that means every other state are disabled, to test multiple states enabled(and other disabled), use | symbol.
ENUM FLAG V2 : to test s state ignoring other states, use (enumVariable & desiredState) == desiredState. combine everything if you want to test multiples states enabled, ignoring other states, exemple : (enumVariable & (stateA | stateB)) == (stateA | stateB)
make an accessor that get itself and serialize it, and getting public while setting private : [field:SerializeField] public float MyAccessor { get; private set; }
scale factor between unity and 3Ds max : 39,37007874015748031496062992126