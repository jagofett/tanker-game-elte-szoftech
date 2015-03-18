package gameserver;

import data.Game;
import data.Direction;
import data.GameState;
import data.Location;
import data.Map;
import data.Player;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.ArrayList;
import java.util.logging.Level;
import java.util.logging.Logger;
import data.KeyboardCommand;
import java.util.HashMap;

public final class GameServer {

    private int port;
    private ServerSocket ss;
    private ArrayList<Connection> connectionList;
    private Game game;
    private GameLogic gameLogic;
    private ArrayList<Integer> playerIDs;
    private HashMap<Integer, Integer> playerIDsWithPositions;

    public GameServer(String port, String[] args) throws IOException {
        this.port = Integer.parseInt(port);
        ss = new ServerSocket(this.port);
        connectionList = new ArrayList<>();

        this.gameLogic = new GameLogic(this);

        playerIDs = new ArrayList<>();
        playerIDs.add(Integer.parseInt(args[0]));
        playerIDs.add(Integer.parseInt(args[1]));
        playerIDs.add(Integer.parseInt(args[2]));
        playerIDs.add(Integer.parseInt(args[3]));

        playerIDsWithPositions = new HashMap<>();
        playerIDsWithPositions.put(playerIDs.get(0), 0);
        playerIDsWithPositions.put(playerIDs.get(1), 1);
        playerIDsWithPositions.put(playerIDs.get(2), 2);
        playerIDsWithPositions.put(playerIDs.get(3), 3);

        runMain();
    }

    public Game getGame() {
        return game;
    }

    public synchronized void removePlayer(Player playerToRemove) {
        game.getGameState().getPlayers().remove(playerToRemove);
        playerIDs.add(playerToRemove.getPlayerNumber());
    }

    public synchronized void removeConnection(Connection connection) {
        connectionList.remove(connection);
    }

    public void runMain() {
        //int playerNumber = 0;
        GameState gameState = new GameState();
        game = new Game(new Map(), gameState);

        System.out.println("* Server is online and waiting for players!");

        while (connectionList.size() < 4) {
            try {
                Socket s = ss.accept();
                ObjectInputStream input = new ObjectInputStream(s.getInputStream());
                ObjectOutputStream output = new ObjectOutputStream(s.getOutputStream());

                String receivedIdAsString = (String) input.readObject();
                Integer receivedID = Integer.parseInt(receivedIdAsString);

                Player player = null;

                if (connectionList.size() + playerIDs.size() == 4 && playerIDs.contains(receivedID)) {
                    switch (playerIDsWithPositions.get(receivedID)) {
                        case 0:
                            player = new Player(receivedID, Direction.RIGHT, new Location(0, 0));
                            break;
                        case 1:
                            player = new Player(receivedID, Direction.DOWN, new Location(190, 0));
                            break;
                        case 2:
                            player = new Player(receivedID, Direction.LEFT, new Location(190, 190));
                            break;
                        case 3:
                            player = new Player(receivedID, Direction.UP, new Location(0, 190));
                            break;
                    }
                    playerIDs.remove(receivedID);
                }

                if (player != null) {
                    connectionList.add(new Connection(s, input, output, player, this));
                    gameState.getPlayers().add(player);
                    System.out.println("Player " + player.getPlayerNumber() + " arrived!");
                }

            } catch (IOException | ClassNotFoundException e) {
                System.out.println("Problem with a player during connecting!");
            }
            System.out.println("* Players connected: " + connectionList.size());

        }

        gameLogic.setGame(game);

        System.out.println("* All players connected, giving them some time to init!");
        try {
            Thread.sleep(5000);
        } catch (InterruptedException ex) {
            Logger.getLogger(GameServer.class.getName()).log(Level.SEVERE, null, ex);
        }

        updateAllClients();

        System.out.println("* Game is starting!");
        gameLogic.setGameIsRunning(true);
    }

    public synchronized void updateAllClients() {
        GameState gameState = game.getGameState();

        for (Connection client : connectionList) {
            try {
                client.update(gameState);
            } catch (IOException ex) {
                Logger.getLogger(GameServer.class.getName()).log(Level.SEVERE, null, ex);
            }
        }
    }

    public synchronized void updatePlayerAboutHisDeath(Player player) {
        for (Connection client : connectionList) {
            if (client.getPlayer() == player) {
                client.sendWinOrLoseMessage("lose");
            }
        }
    }

    public synchronized void updatePlayerAboutHisWinning(Player player) {
        for (Connection client : connectionList) {
            if (client.getPlayer() == player) {
                client.sendWinOrLoseMessage("win");
            }
        }
    }

    public synchronized void sendCommandToLogic(KeyboardCommand keyboardCommand, Player player) {
        gameLogic.interpretKeyboardCommand(keyboardCommand, player);
    }

}
