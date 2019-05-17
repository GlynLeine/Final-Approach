String RetrieveMessage()
{
  digitalID = 0;
  analogID = 0;

  String message;
  while (Serial.available() > 0)
  {
    char c = Serial.read();
    message.concat(c);
  }
  return message;
}
