int outputs [] = { A1, A2, A3, A4, A5 };
int number_of_outputs = 5;
char char_for_out1 = '1';
unsigned long pulse_length_microseconds = 50000;
unsigned long times_to_turn_off[] = { 0, 0, 0, 0, 0 };

void setup()
{
  Serial.begin( 9600 );
  for( int i = 0; i < number_of_outputs; i++ )
  {
    pinMode( outputs[ i ], OUTPUT );
  }  
}

void loop()
{
  while( Serial.available() > 0 )
  {
    byte b = Serial.read();
    Serial.flush();
    // When a '?' character is sent, return a string that indicates the firmware
    if( b == '?' )
    {
      Serial.println( "BAMBAM_V01" );
    }
    
    // When a '1' character is sent
    else if( b >= char_for_out1 && b < char_for_out1 + number_of_outputs )
    {
      // Convert charater '1' to integer 0, '2' to 1,...
      int i = b - char_for_out1;
      
      // Which output should be set?
      int output = outputs[ i ];
      digitalWrite( output, 1 );
      
      // When should the output be reset again?
      times_to_turn_off[ i ] = micros() + pulse_length_microseconds;
      
      // Send back some text info
      Serial.print( "Bam -> " );
      Serial.print( output );
      Serial.println();
    }
    
    // Unknown command
    else
    {
      Serial.println( ":-?" );
    }
    
  }
  
  // Reset outputs (if any)
  for( int i = 0; i < number_of_outputs; i++ )
  {
    if( times_to_turn_off[ i ] < micros() )
    {
      digitalWrite(outputs[ i ], 0);
    }
  }
}
