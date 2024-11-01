access all tags in project : UnityEditorInternal.InternalEditorUtility.tags
check null value of a native class instance : ReferenceEquals(Class instance, null)    PS : ReferenceEquals(A, B) test a real reference test 100% solid between A and B
make an enum flag : to make a flag of selectable like a layerMask, put the enum definition with [System.Flags] and define numbers to each values spaced enough to not make glitches
ENUM FLAG : by default, test if a specific state is enabled, that means every other state are disabled, to test multiple states enabled(and other disabled), use | symbol.
ENUM FLAG V2 : to test s state ignoring other states, use (enumVariable & desiredState) == desiredState. combine everything if you want to test multiples states enabled, ignoring other states, exemple : (enumVariable & (stateA | stateB)) == (stateA | stateB)
make an accessor that get itself and serialize it, and getting public while setting private : [field:SerializeField] public float MyAccessor { get; private set; }
scale factor between unity and 3Ds max : 39,37007874015748031496062992126
to format a variable name to be displayed, use ObjectNames.NicifyVariableName(string variableName)
informations about basic recting of layout of serialized fields : above 335 pixels, xMin bound of fields are precisely at a lerp of 0.45 of the total width of inspector window, minus 30 pixels, wich is calculated and written : Rect.NormalizedToPoint(position, new Vector2(0.45f, 0)).x - 30, 
Same is for max bound, that is precisely at a lerp of 0.815 of the total width of inspector window, minus 10 pixels, written : Rect.NormalizedToPoint(position, new Vector2(0.815f, 0)).x - 10