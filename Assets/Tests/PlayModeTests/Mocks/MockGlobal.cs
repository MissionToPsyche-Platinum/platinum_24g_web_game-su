namespace PlayModeTests.Mocks
{
    public class MockGlobal
    {
        public int Round { get; set; }
        public int TotalScore { get; set; } = 0;
        public string CurrentRoom { get; set; } = "";
        public bool CurrentRoomCompleted { get; set; } = false;
    }
}