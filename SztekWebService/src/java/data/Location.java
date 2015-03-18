package data;

import java.io.Serializable;

public final class Location implements Serializable {

    private int x;
    private int y;
    private boolean isValidLocation;

    public Location(int x, int y) {
        this.x = x;
        this.y = y;
        this.isValidLocation = true;
    }

    public int getX() {
        return x;
    }

    public int getY() {
        return y;
    }

    public void setX(int x) {
        if (x >= 0 && x <= 190) {
            this.x = x;
        } else {
            this.isValidLocation = false;
        }
    }

    public void setY(int y) {
        if (y >= 0 && y <= 190) {
            this.y = y;
        } else {
            this.isValidLocation = false;
        }
    }

    public boolean isIsValidLocation() {
        return isValidLocation;
    }

    @Override
    public boolean equals(Object obj) {
        if (obj instanceof Location) {
            Location otherLocation = (Location) obj;

            if (this.getX() == otherLocation.getX() && this.getY() == otherLocation.getY()) {
                return true;
            }
        }

        return false;
    }

    @Override
    public int hashCode() {
        int hash = 7;
        hash = 83 * hash + this.x;
        hash = 83 * hash + this.y;
        return hash;
    }

}
