package gameserver;

import data.GameState;
import data.KeyboardCommand;
import data.Player;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.net.Socket;
import java.util.logging.Level;
import java.util.logging.Logger;

public final class Connection {

    private final Socket s;
    private final ObjectInputStream fromClientStream;
    private final ObjectOutputStream toClientStream;
    private final Player player;
    private final GameServer gameServer;

    public Connection(Socket s, ObjectInputStream input, ObjectOutputStream output, Player player, GameServer gameServer) {
        this.s = s;
        this.fromClientStream = input;
        this.toClientStream = output;
        this.player = player;
        this.gameServer = gameServer;

        runMyInputHandling();

    }

    public ObjectInputStream getInput() {
        return fromClientStream;
    }

    public ObjectOutputStream getOutput() {
        return toClientStream;
    }

    public Player getPlayer() {
        return player;
    }
    
    public Connection getMe() {
        return this;
    }

    public void runMyInputHandling() {
        new Thread(new Runnable() {
            @Override
            public void run() {
                ObjectInputStream myInput = getInput();
                while (!s.isClosed()) {
                    try {
                        KeyboardCommand receivedCommand = (KeyboardCommand) myInput.readObject();
                        gameServer.sendCommandToLogic(receivedCommand, player);
                    } catch (IOException | ClassNotFoundException ex) {
                        synchronized (Connection.this) {
                            try {
                                s.close();
                            } catch (IOException ex1) {
                                Logger.getLogger(Connection.class.getName()).log(Level.SEVERE, null, ex1);
                            }
                            gameServer.removePlayer(player);
                            gameServer.removeConnection(getMe());
                            System.out.println("Problem with player " + player.getPlayerNumber() + "'s connection! Terminating connection..");
                        }
                    }
                }
            }
        }).start();
    }

    public synchronized void update(GameState gameState) throws IOException {
        toClientStream.writeObject(gameState);
        toClientStream.flush();
        toClientStream.reset();
    }
    
    public synchronized void sendWinOrLoseMessage(String message) {
        try {
            toClientStream.writeObject(message);
            toClientStream.flush();
            toClientStream.reset();
        } catch (IOException ex) {
            Logger.getLogger(Connection.class.getName()).log(Level.SEVERE, null, ex);
        }
    }

}
