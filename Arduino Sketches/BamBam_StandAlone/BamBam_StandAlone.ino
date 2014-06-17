int outputs [] = { A1, A2, A3, A4, A5 };
int const number_of_outputs = 5;
int const rhythm_length = 16;

int rhythm[ number_of_outputs ][ rhythm_length ] =
{
  { 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0 },
  { 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0 },
  { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 },
  { 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1 },
  { 0, 0, 0, 1, 0, 0, 1, 1, 0, 1, 0, 1, 0, 0, 1, 0 },
};

int pulse_on_milli = 50;
int pulse_off_milli = 150;
int rhythm_step = 0;

void setup()
{
  for( int i = 0; i < number_of_outputs; i++ )
  {
    pinMode( outputs[ i ], OUTPUT );
  }  
}

void loop()
{
  for( int i = 0; i < number_of_outputs; i++ )
  {
    digitalWrite( outputs[ i ], rhythm[ i ][ rhythm_step ] );
  }
  delay( pulse_on_milli );
  
  for( int i = 0; i < number_of_outputs; i++ )
  {
    digitalWrite( outputs[ i ], 0 );
  }
  delay( pulse_off_milli );

  rhythm_step = ( rhythm_step + 1 ) % rhythm_length;
}
