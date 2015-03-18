package data;

import java.io.Serializable;

public class Player implements Serializable {

    private final int playerNumber;
    private boolean alive;
    private Direction direction;
    private boolean moving;
    private Location location;

    public Player(int playerNumber, Direction direction, Location location) {
        this.playerNumber = playerNumber;
        this.direction = direction;
        this.location = location;
        this.alive = true;
        this.moving = false;
    }

    public int getPlayerNumber() {
        return playerNumber;
    }

    public boolean isAlive() {
        return alive;
    }

    public Direction getDirection() {
        return direction;
    }

    public boolean isMoving() {
        return moving;
    }

    public Location getLocation() {
        return location;
    }

    public void setAlive(boolean isAlive) {
        this.alive = isAlive;
    }

    public void setDirection(Direction direction) {
        this.direction = direction;
    }

    public void setMoving(boolean isMoving) {
        this.moving = isMoving;
    }

    public void setLocation(Location location) {
        this.location = location;
    }

}
