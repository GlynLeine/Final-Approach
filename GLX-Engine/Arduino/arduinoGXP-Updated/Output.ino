int ReceiveCheck(String& message)
{
  if (message.indexOf("RECEIVE") >= 0)
  {
    int outputID = message.substring(message.indexOf('#')+1, message.indexOf("=>")-1).toInt();
    message = message.substring(message.indexOf("=>") + 3);
    return outputID;
  }
  return 0;
}

void ActuateServo(int servoID, String& message)
{
  int targetAngle = message.substring(0, message.indexOf(';')).toInt();
  int currentAngle = servoAngles[servoID-1];
  int rotation;
  if(targetAngle<currentAngle)
    rotation = -1;
  else
    rotation = 1;
  
  for(int i = servoAngles[servoID-1]; i!= targetAngle + rotation; i+= rotation)
  {
    servos[servoID-1].write(i);
    Serial.println(i);
    delay(2.5);
  }

  servoAngles[servoID-1] = targetAngle;
}
