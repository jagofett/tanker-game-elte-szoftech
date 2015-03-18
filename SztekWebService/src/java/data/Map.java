package data;

import java.util.ArrayList;

public class Map {

    private ArrayList<Location> walls;

    public Map() {
        this.walls = new ArrayList<>();
        thisMapWillBeGoodForNow();
    }

    public ArrayList<Location> getWalls() {
        return walls;
    }

    private void thisMapWillBeGoodForNow() {
        this.walls.add(new Location(50, 0));
        this.walls.add(new Location(60, 0));
        this.walls.add(new Location(90, 0));
        this.walls.add(new Location(100, 0));
        this.walls.add(new Location(130, 0));
        this.walls.add(new Location(140, 0));

        this.walls.add(new Location(90, 10));
        this.walls.add(new Location(100, 10));

        this.walls.add(new Location(20, 20));
        this.walls.add(new Location(30, 20));
        this.walls.add(new Location(160, 20));
        this.walls.add(new Location(170, 20));

        this.walls.add(new Location(20, 30));
        this.walls.add(new Location(30, 30));
        this.walls.add(new Location(160, 30));
        this.walls.add(new Location(170, 30));

        this.walls.add(new Location(0, 50));
        this.walls.add(new Location(50, 50));
        this.walls.add(new Location(60, 50));
        this.walls.add(new Location(70, 50));
        this.walls.add(new Location(80, 50));
        this.walls.add(new Location(110, 50));
        this.walls.add(new Location(120, 50));
        this.walls.add(new Location(130, 50));
        this.walls.add(new Location(140, 50));
        this.walls.add(new Location(190, 50));

        this.walls.add(new Location(0, 60));
        this.walls.add(new Location(50, 60));
        this.walls.add(new Location(140, 60));
        this.walls.add(new Location(190, 60));

        this.walls.add(new Location(50, 70));
        this.walls.add(new Location(140, 70));

        this.walls.add(new Location(50, 80));
        this.walls.add(new Location(140, 80));

        this.walls.add(new Location(0, 90));
        this.walls.add(new Location(10, 90));
        this.walls.add(new Location(180, 90));
        this.walls.add(new Location(190, 90));

        this.walls.add(new Location(0, 100));
        this.walls.add(new Location(10, 100));
        this.walls.add(new Location(180, 100));
        this.walls.add(new Location(190, 100));

        this.walls.add(new Location(50, 110));
        this.walls.add(new Location(140, 110));

        this.walls.add(new Location(50, 120));
        this.walls.add(new Location(140, 120));

        this.walls.add(new Location(0, 130));
        this.walls.add(new Location(50, 130));
        this.walls.add(new Location(140, 130));
        this.walls.add(new Location(190, 130));

        this.walls.add(new Location(0, 140));
        this.walls.add(new Location(50, 140));
        this.walls.add(new Location(60, 140));
        this.walls.add(new Location(70, 140));
        this.walls.add(new Location(80, 140));
        this.walls.add(new Location(110, 140));
        this.walls.add(new Location(120, 140));
        this.walls.add(new Location(130, 140));
        this.walls.add(new Location(140, 140));
        this.walls.add(new Location(190, 140));

        this.walls.add(new Location(20, 160));
        this.walls.add(new Location(30, 160));
        this.walls.add(new Location(160, 160));
        this.walls.add(new Location(170, 160));

        this.walls.add(new Location(20, 170));
        this.walls.add(new Location(30, 170));
        this.walls.add(new Location(160, 170));
        this.walls.add(new Location(170, 170));

        this.walls.add(new Location(90, 180));
        this.walls.add(new Location(100, 180));

        this.walls.add(new Location(50, 190));
        this.walls.add(new Location(60, 190));
        this.walls.add(new Location(90, 190));
        this.walls.add(new Location(100, 190));
        this.walls.add(new Location(130, 190));
        this.walls.add(new Location(140, 190));

    }
}
