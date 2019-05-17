/*
  Send data to GXP

  based on code by Eusebiu Mihail Buga
  created 10 Feb 2019
  
  Final build by Glyn Marcus Leine
    - redone Send and Receive
    - re-organised code
    - added ID and I/O tracking

  For the Arduino Leonardo, Micro or Due

  The circuit:
  - as long as it sends something to the serial port it can even be a gravity gun built with scavenged MOSFETs


*/
#include <Servo.h>
Servo servos[2];
int servoAngles[2];

int analogs = 0;
int digitals = 0;
int analogID = 0;
int digitalID = 0;
int ID;

bool initialiseRun = true;
bool connectedToEngine = false;

/* leonardo digital pins:
 * Servo start: 11
 * Servo end: 9
 * Start button: 7
 * Cog button: 5
 */

void setup()
{
  // open the serial port:
  Serial.begin(9600);
  Serial.setTimeout(5000);

  pinMode(5, INPUT_PULLUP);
  pinMode(7, INPUT_PULLUP);
  pinMode(9, OUTPUT);
  pinMode(11, OUTPUT);

  servos[0].attach(9);
  servos[1].attach(11);
  servoAngles[0] = 0;
  servoAngles[1] = 0;

  while (!Serial)
  {
    ;
  }

  randomSeed(analogRead(0));
  ID = random(32767);

  loop();
  initialiseRun = false;
}

void loop()
{
  String message = RetrieveMessage();
  if (CheckHandshake(message) || initialiseRun)
  {
    SendDigitalData(5);
    SendDigitalData(7);
  }

  int receiveOutput = ReceiveCheck(message);  
  if(receiveOutput > 0)
  {
    ActuateServo(receiveOutput, message);
  }
}
