package gameserver;

import javax.jws.WebMethod;
import javax.jws.WebService;
import java.io.IOException;
import java.util.logging.Level;
import java.util.logging.Logger;

@WebService(serviceName = "Main")
public class Main {

    @WebMethod(operationName = "startGameServer")
    public String startGameServer(String serverPort, String gameId, String id1, String id2, String id3, String id4) throws IOException {

        String[] IPs = {id1, id2, id3, id4} ;
        
        new Thread(new Runnable() {
            @Override
            public void run() { 
                try {
                    GameServer gameServer = new GameServer(serverPort, gameId, IPs);
                } catch (IOException ex) {
                    Logger.getLogger(Main.class.getName()).log(Level.SEVERE, null, ex);
                }
            }
        }).start();
        
        return "gameServerInstance started";
    }
    

}
