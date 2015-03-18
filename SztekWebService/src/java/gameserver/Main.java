package gameserver;

import javax.jws.WebMethod;
import javax.jws.WebService;
import java.io.IOException;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.jws.WebParam;

@WebService(serviceName = "Main")
public class Main {

    @WebMethod(operationName = "startGameServer")
    public String startGameServer(@WebParam(name = "serverPort") final String serverPort,
                                @WebParam(name = "ip1") String ip1, 
                                @WebParam(name = "ip2") String ip2, 
                                @WebParam(name = "ip3") String ip3, 
                                @WebParam(name = "ip4") String ip4) throws IOException {

        final String[] IPs = {ip1, ip2, ip3, ip4} ;
        
        new Thread(new Runnable() {
            @Override
            public void run() {
                try {
                    GameServer gameServer = new GameServer(serverPort, IPs);
                } catch (IOException ex) {
                    Logger.getLogger(Main.class.getName()).log(Level.SEVERE, null, ex);
                }
            }
        }).start();
        
        return "gameServerInstance started";
    }
    

}
