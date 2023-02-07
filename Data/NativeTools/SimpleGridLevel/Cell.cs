namespace UPDB.Data.NativeTools.SimpleGridLevel
{
    ///<summary>
    /// represent cells epxloitable into a 2D grid
    ///</summary>
    public class Cell
    {
        #region Variables

        private Coords2D _position;

        #endregion

        #region public API

        public Coords2D Position => _position;

        #endregion

        public Cell(int x, int y)
        {
            _position = new Coords2D(x, y);
        }
    } 
}
