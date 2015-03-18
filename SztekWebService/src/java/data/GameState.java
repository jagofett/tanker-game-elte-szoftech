package data;

import java.io.Serializable;
import java.util.ArrayList;

public class GameState implements Serializable {

    private ArrayList<Player> players;
    private ArrayList<Projectile> projectiles;

    public GameState() {
        this.players = new ArrayList<>();
        this.projectiles = new ArrayList<>();
    }

    public ArrayList<Player> getPlayers() {
        return players;
    }

    public ArrayList<Projectile> getProjectiles() {
        return projectiles;
    }

}
