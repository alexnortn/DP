import processing.core.*; 
import processing.data.*; 
import processing.event.*; 
import processing.opengl.*; 

import oscP5.*; 

import oscP5.*; 
import netP5.*; 

import java.util.HashMap; 
import java.util.ArrayList; 
import java.io.File; 
import java.io.BufferedReader; 
import java.io.PrintWriter; 
import java.io.InputStream; 
import java.io.OutputStream; 
import java.io.IOException; 

public class oscP5test extends PApplet {



/**
 * simple example of creating a new instance
 * of oscP5 to send and receive osc messages.
 */
 
OscP5 oscP5;

int pitch_ ;
int volume_ ;

// a NetAddress contains the ip address and port number of a remote location in the network.
NetAddress myRemoteLocation; 

public void setup() {
  size(400,400);
  frameRate(25);
  // create a new instance of oscP5. 
  // 7400 is the port number you are listening for incoming osc messages.
  oscP5 = new OscP5(this,7400);
  
  // create a new NetAddress. a NetAddress is used when sending osc messages
  // with the oscP5.send method.
  myRemoteLocation = new NetAddress("127.0.0.1",7400);
  
  // with the plug method of oscP5 you can automatically forward
  // incoming osc messages with a specific address pattern (3rd parameter in the plug method)
  // and a specific typetag (typetag=parameters of plugged method) to a
  // specific method (2nd parameter) in your sketch.
  oscP5.plug(this,"test","/test");
}


public void test(int theR, int theG, int theB) {
    int redMap = constrain(PApplet.parseInt(map(theR, 0, 35, 0, 255)), 0, 255);
    int greenMap = constrain(PApplet.parseInt(map(theG, 40, 100, 0, 255)), 0, 255);

    println("volume_map = " + redMap + "   pitch_map = " + greenMap);

  volume_ = color(redMap,0,0);
  pitch_ = color(0,theG,0);
}

public void draw() {
  background(0);
  noStroke();
  fill(pitch_);
  rect(0,0,400,200);
  fill(volume_);
  rect(0,200,400,200);
}


public void mousePressed() {
  // create a new OscMessage with an address pattern, in this case /test.
  OscMessage myOscMessage = new OscMessage("/mousexy");
  // add a value (an integer) to the OscMessage
  myOscMessage.add((int)mouseX);
  myOscMessage.add((int)mouseY);
  // send the OscMessage to a remote location specified in myNetAddress
  oscP5.send(myOscMessage, myRemoteLocation);
}


// incoming osc message are forwarded to the oscEvent method.
// void oscEvent(OscMessage theOscMessage) {
//   // get and print the address pattern and the typetag of the received OscMessage
//   println("### received an osc message with addrpattern "+theOscMessage.addrpattern()+" and typetag "+theOscMessage.typetag());
//   theOscMessage.print();
// }
  static public void main(String[] passedArgs) {
    String[] appletArgs = new String[] { "--full-screen", "--bgcolor=#666666", "--stop-color=#cccccc", "oscP5test" };
    if (passedArgs != null) {
      PApplet.main(concat(appletArgs, passedArgs));
    } else {
      PApplet.main(appletArgs);
    }
  }
}
