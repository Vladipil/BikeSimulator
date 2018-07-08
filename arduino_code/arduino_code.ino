#include <EEPROM.h>

const int ledPin = 13; //LED_BUILTIN
const int vibratePin = 12;
const int hallPin = 2;
const int grayScalePin = 0;

volatile unsigned long lastturn, time_press;
volatile float SPEED;
volatile float DIST;
volatile boolean eeprom_flag;
float w_length = 2.2;
boolean flag;

char data = 0;                //Variable for storing received data
int turnValue = 0;            //Turn value (randing from 100 to 260)
int lastTurnValue = 0;


void calcSpeed()
{
  if ((millis() - lastturn) > 2000 && SPEED != 0)         // if there is no signal more than 2 seconds
  {       
    SPEED = 0;                              // so, speed is 0
    sendLine("0", String(SPEED));
    sendLine("1", String(DIST));
    if (eeprom_flag) 
    {                                       // if eeprom flag is true
      EEPROM.write(0, (float)DIST * 10.0);  // write ODO in EEPROM
      eeprom_flag = 0;                      // flag down. To prevent rewritind
    }
  }
}

void received() 
{
  if(Serial.available() > 0)            // Send data only when you receive data:
  {
    data = Serial.read();               //Read the incoming data and store it into variable data
    if(data == '1')                     //Checks whether value of data is equal to 1 
    {
      detachInterrupt(digitalPinToInterrupt(hallPin));
      
      digitalWrite(vibratePin, HIGH);   //If value is 1 then vibratePin turns ON
      SPEED = 0;
      sendLine("0", String(SPEED));
      delay(600);
      digitalWrite(vibratePin, LOW);    //If value is 0 then vibratePin turns OFF
      
      attachInterrupt(digitalPinToInterrupt(hallPin), hall_interrupt, RISING); // hall sensor interrupt
    }
  }  
}

void calcTurn() 
{
  turnValue = analogRead(grayScalePin);            //connect grayscale sensor to Analog 0
  int dif = lastTurnValue - turnValue;
  if(abs(dif) > 3)
  {
    lastTurnValue = turnValue;
    sendLine("2", String(turnValue));
  }
}

void hall_interrupt() 
{
  if (millis() - lastturn > 80)             // simple noise cut filter (based on fact that you will not be ride your bike more than 120 km/h =)
  {    
    SPEED = w_length / ((float)(millis() - lastturn) / 1000) * 3.6;   // calculate speed
    lastturn = millis();                                              // remember time of last revolution
    DIST = DIST + w_length / 1000;                                    // calculate distance
  }
  sendLine("0", String(SPEED));
  sendLine("1", String(DIST));
  eeprom_flag = 1;
  digitalWrite(ledPin, HIGH);
  delay(1);
  digitalWrite(ledPin, LOW);
}

void setup()
{
  pinMode(ledPin, OUTPUT);
  pinMode(vibratePin, OUTPUT);
  pinMode(hallPin, INPUT_PULLUP);
  Serial.begin( 9600 );
  attachInterrupt(digitalPinToInterrupt(hallPin), hall_interrupt, RISING); // hall sensor interrupt
}

void loop()
{
    received();
    calcSpeed();
    calcTurn();
}

void sendLine(String code, String value)
{
  Serial.print("(");
  Serial.print(code);
  Serial.print(":");
  Serial.print(value);
  Serial.println(")"); 
}
