package data;

public class Game {

    private final Map map;
    private GameState gameState;

    public Game(Map map, GameState gameState) {
        this.map = map;
        this.gameState = gameState;
    }

    public Map getMap() {
        return map;
    }

    public GameState getGameState() {
        return gameState;
    }

    public void setGameState(GameState gameState) {
        this.gameState = gameState;
    }

}
