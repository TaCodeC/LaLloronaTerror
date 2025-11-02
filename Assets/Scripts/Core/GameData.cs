    public static class GameData
    {
        public enum ItemType : int { Battery , Pliers , Flashlight , Key , CameraBattery, Interaction  }
        public enum Interactables: int {None,Door, Gate, LockedDoor}
        
        public enum Objective { NONE, ENERGY, DOOR, CONTROL, ESCAPE }
        public enum demiObjective {SEARCH, USE};
        public enum GameState { PLAYING, PAUSE, GAMEOVER }
        public enum LloronaState {STAY, ALERT, RUNNING, STUN }
    }