package data;

import java.io.Serializable;

public class Projectile implements Serializable {

    private Direction direction;
    private Location location;

    public Projectile(Location location, Direction direction) {
        this.location = location;
        this.direction = direction;
    }

    public Direction getDirection() {
        return direction;
    }

    public Location getLocation() {
        return location;
    }

    public void setLocation(Location location) {
        this.location = location;
    }

}
