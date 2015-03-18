package data;

import java.io.Serializable;

public class KeyboardCommand implements Serializable {

    public int keyCode;
    public boolean pressedOrReleased;

    public KeyboardCommand(int keyCode, boolean pressedOrReleased) {
        this.keyCode = keyCode;
        this.pressedOrReleased = pressedOrReleased;
    }

}
