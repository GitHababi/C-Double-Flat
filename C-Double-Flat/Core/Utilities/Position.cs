namespace C_Double_Flat.Core.Utilities
{
    public struct Position
    {
        public int Row;
        public int Column;
        public int Index;

        public Position(int row, int column, int index)
        {
            Row = row;
            Column = column;
            Index = index;
        }
        public static readonly Position Zero = new Position(0, 0, 0);
    }
}
