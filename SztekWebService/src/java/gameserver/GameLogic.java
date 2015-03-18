package gameserver;

import data.Direction;
import data.Game;
import data.KeyboardCommand;
import data.Location;
import data.Player;
import data.Projectile;
import static java.awt.event.KeyEvent.VK_DOWN;
import static java.awt.event.KeyEvent.VK_LEFT;
import static java.awt.event.KeyEvent.VK_RIGHT;
import static java.awt.event.KeyEvent.VK_SPACE;
import static java.awt.event.KeyEvent.VK_UP;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.URLConnection;
import java.sql.Driver;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.Properties;
import java.util.logging.Level;
import java.util.logging.Logger;

public class GameLogic {

    private Game game;
    private GameServer gameServer;
    private boolean gameIsRunning;
    private int gameEndingCounter;

    public GameLogic(GameServer gameServer) {
        this.gameServer = gameServer;
        this.gameIsRunning = false;
        this.gameEndingCounter = 0;

        new Thread(new Runnable() {
            @Override
            public void run() {

                while (!gameIsRunning) {
                    try {
                        Thread.sleep(100);
                    } catch (InterruptedException ex) {
                        Logger.getLogger(GameLogic.class.getName()).log(Level.SEVERE, null, ex);
                    }
                }

                while (gameIsRunning) {
                    long startTime = System.nanoTime();

                    calculateGameState();

                    updateAllClientsInGameServer();

                    long timeToSleepInMillis = 17 - (System.nanoTime() - startTime) / 1000000;
                    try {
                        if (timeToSleepInMillis > 0) {
                            Thread.sleep(timeToSleepInMillis);
                        }
                    } catch (InterruptedException ex) {
                        Logger.getLogger(GameLogic.class.getName()).log(Level.SEVERE, null, ex);
                    }
                }
                
                System.out.println("* Everything went well, I am shutting down.");
            }
        }).start();
    }

    public void setGame(Game game) {
        this.game = game;
    }

    public void setGameIsRunning(boolean gameIsRunning) {
        this.gameIsRunning = gameIsRunning;
    }

    synchronized private void calculateGameState() {
        ArrayList<Player> players = game.getGameState().getPlayers();
        ArrayList<Projectile> projectiles = game.getGameState().getProjectiles();
        ArrayList<Location> walls = game.getMap().getWalls();

        projectileWallCollision(walls, projectiles);
        playerProjectileCollision(players, projectiles);
        playerWallAndOtherPlayerlCollision(players, walls);

        updateProjectiles(projectiles);
        updatePlayers(players);

        if (gameIsRunning && getAlivePlayers() <= 1) {
            ++gameEndingCounter;
        }

        if (gameIsRunning && gameEndingCounter >= 120) {
            gameIsRunning = false;
            checkWinner();
        }

    }

    public synchronized void interpretKeyboardCommand(KeyboardCommand keyboardCommand, Player player) {
        int keyCode = keyboardCommand.keyCode;
        boolean pressedOrReleased = keyboardCommand.pressedOrReleased;

        if (player == null || !player.isAlive()) {
            return;
        }

        if (pressedOrReleased) {
            switch (keyCode) {
                case VK_UP:
                    if (player.getDirection() == Direction.UP) {
                        player.setMoving(true);
                    } else {
                        player.setDirection(Direction.UP);
                    }
                    break;
                case VK_DOWN:
                    if (player.getDirection() == Direction.DOWN) {
                        player.setMoving(true);
                    } else {
                        player.setDirection(Direction.DOWN);
                    }
                    break;
                case VK_LEFT:
                    if (player.getDirection() == Direction.LEFT) {
                        player.setMoving(true);
                    } else {
                        player.setDirection(Direction.LEFT);
                    }
                    break;
                case VK_RIGHT:
                    if (player.getDirection() == Direction.RIGHT) {
                        player.setMoving(true);
                    } else {
                        player.setDirection(Direction.RIGHT);
                    }
                    break;
                case VK_SPACE:
                    shoot(player);
                    break;
            }
        } else {
            Direction playerDirection = player.getDirection();
            boolean isPlayerMoving = player.isMoving();
            switch (keyCode) {
                case VK_UP:
                    if (isPlayerMoving && playerDirection == Direction.UP) {
                        player.setMoving(false);
                    }
                    break;
                case VK_DOWN:
                    if (isPlayerMoving && playerDirection == Direction.DOWN) {
                        player.setMoving(false);
                    }
                    break;
                case VK_LEFT:
                    if (isPlayerMoving && playerDirection == Direction.LEFT) {
                        player.setMoving(false);
                    }
                    break;
                case VK_RIGHT:
                    if (isPlayerMoving && playerDirection == Direction.RIGHT) {
                        player.setMoving(false);
                    }
                    break;
            }
        }

    }

    private synchronized void shoot(Player player) {
        Direction playerDirection = player.getDirection();
        Location playerLocation = player.getLocation();

        Projectile projectile = new Projectile(new Location(0, 0), playerDirection);

        switch (playerDirection) {
            case UP:
                projectile.getLocation().setX(playerLocation.getX());
                projectile.getLocation().setY(playerLocation.getY() - 10);
                game.getGameState().getProjectiles().add(projectile);
                break;
            case DOWN:
                projectile.getLocation().setX(playerLocation.getX());
                projectile.getLocation().setY(playerLocation.getY() + 10);
                game.getGameState().getProjectiles().add(projectile);
                break;

            case LEFT:
                projectile.getLocation().setX(playerLocation.getX() - 10);
                projectile.getLocation().setY(playerLocation.getY());
                game.getGameState().getProjectiles().add(projectile);
                break;
            case RIGHT:
                projectile.getLocation().setX(playerLocation.getX() + 10);
                projectile.getLocation().setY(playerLocation.getY());
                game.getGameState().getProjectiles().add(projectile);
                break;
        }
    }

    private synchronized void updateAllClientsInGameServer() {
        gameServer.updateAllClients();
    }

    private synchronized void playerWallAndOtherPlayerlCollision(ArrayList<Player> players, ArrayList<Location> walls) {

        for (Player player : players) {

            if (player.isAlive() && player.isMoving()) {

                Direction playerDirection = player.getDirection();
                Location targetLocation = new Location(0, 0);

                switch (playerDirection) {
                    case UP:
                        targetLocation.setX(player.getLocation().getX());
                        targetLocation.setY(player.getLocation().getY() - 10);
                        break;
                    case DOWN:
                        targetLocation.setX(player.getLocation().getX());
                        targetLocation.setY(player.getLocation().getY() + 10);
                        break;
                    case LEFT:
                        targetLocation.setX(player.getLocation().getX() - 10);
                        targetLocation.setY(player.getLocation().getY());
                        break;
                    case RIGHT:
                        targetLocation.setX(player.getLocation().getX() + 10);
                        targetLocation.setY(player.getLocation().getY());
                        break;
                }

                if (targetLocation.isIsValidLocation()) {
                    for (Location wall : walls) {
                        if (targetLocation.equals(wall)) {
                            player.setMoving(false);
                            break;
                        }
                    }

                    for (Player otherPlayer : players) {
                        if (otherPlayer.isAlive() && targetLocation.equals(otherPlayer.getLocation())) {
                            player.setMoving(false);
                            break;
                        }
                    }
                }
            }

        }

    }

    private synchronized void playerProjectileCollision(ArrayList<Player> players, ArrayList<Projectile> projectiles) {
        ArrayList<Projectile> wrongProjectiles = new ArrayList<>();

        for (Player player : players) {
            if (player.isAlive()) {
                for (Projectile projectile : projectiles) {
                    if (player.getLocation().equals(projectile.getLocation())) {
                        player.setAlive(false);
                        gameServer.updatePlayerAboutHisDeath(player);
                        player.setMoving(false);
                        wrongProjectiles.add(projectile);
                        break;
                    }
                }
            }
        }

        for (Projectile projectile : wrongProjectiles) {
            projectiles.remove(projectile);
        }
    }

    private synchronized void projectileWallCollision(ArrayList<Location> walls, ArrayList<Projectile> projectiles) {
        ArrayList<Projectile> wrongProjectiles = new ArrayList<>();

        for (Projectile projectile : projectiles) {
            for (Location wall : walls) {
                if (projectile.getLocation().equals(wall) || !projectile.getLocation().isIsValidLocation()) {
                    wrongProjectiles.add(projectile);
                    break;
                }
            }
        }

        for (Projectile projectile : wrongProjectiles) {
            projectiles.remove(projectile);
        }

    }

    private synchronized void updateProjectiles(ArrayList<Projectile> projectiles) {
        for (Projectile projectile : projectiles) {
            switch (projectile.getDirection()) {
                case UP:
                    projectile.getLocation().setY(projectile.getLocation().getY() - 5);
                    break;
                case DOWN:
                    projectile.getLocation().setY(projectile.getLocation().getY() + 5);
                    break;
                case LEFT:
                    projectile.getLocation().setX(projectile.getLocation().getX() - 5);
                    break;
                case RIGHT:
                    projectile.getLocation().setX(projectile.getLocation().getX() + 5);
                    break;
            }
        }
    }

    private synchronized void updatePlayers(ArrayList<Player> players) {
        for (Player player : players) {
            if (player.isMoving()) {
                switch (player.getDirection()) {
                    case UP:
                        player.getLocation().setY(player.getLocation().getY() - 10);
                        break;
                    case DOWN:
                        player.getLocation().setY(player.getLocation().getY() + 10);
                        break;
                    case LEFT:
                        player.getLocation().setX(player.getLocation().getX() - 10);
                        break;
                    case RIGHT:
                        player.getLocation().setX(player.getLocation().getX() + 10);
                        break;
                }

                // TEMPORARY SOLUTION
                player.setMoving(false);
            }
        }
    }

    private synchronized void checkWinner() {
        if (getAlivePlayers() == 1) {
            Player winner = getLastAlivePlayer();

            System.out.println("* Player " + winner.getPlayerNumber() + " has won the game!");
            gameServer.updatePlayerAboutHisWinning(winner);
            writeWinnerToDatabase(winner.getPlayerNumber());
            return;
        }
        if (getAlivePlayers() == 0) {
            System.out.println("* There are no winners!");
        }

    }

    public synchronized void writeWinnerToDatabase(int winnerId) {

        try {
            final URL url = new URL("http://www.tanker.somee.com/Home/EndGameResult");
            HttpURLConnection myConnection = (HttpURLConnection) url.openConnection();
            myConnection.setRequestMethod("POST");
            myConnection.setDoOutput(true);
            
            DataOutputStream wr = new DataOutputStream(myConnection.getOutputStream ());
            
            wr.writeBytes("gameId=" + this.gameServer.gameId + "&winnerId=" + winnerId);
        } catch (MalformedURLException ex) {
            Logger.getLogger(GameLogic.class.getName()).log(Level.SEVERE, null, ex);
        } catch (IOException ex) {
            Logger.getLogger(GameLogic.class.getName()).log(Level.SEVERE, null, ex);
        }

    }

    public synchronized int getAlivePlayers() {
        int alivePlayers = 0;
        for (Player player : game.getGameState().getPlayers()) {
            if (player.isAlive()) {
                ++alivePlayers;
            }
        }
        return alivePlayers;
    }

    public synchronized Player getLastAlivePlayer() {
        for (Player player : game.getGameState().getPlayers()) {
            if (player.isAlive()) {
                return player;
            }
        }
        return null;
    }
}
