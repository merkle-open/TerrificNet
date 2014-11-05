lexer grammar MustacheLexer;

ESC_VAR
  : {varEscape(start, end)}? .
  ;

TEXT
  : {consumeUntil(start)}? .
  ;

COMMENT
  : {comment(start, end)}? .
  ;

START_AMP
 : {startToken(start, "&")}? . -> pushMode(VAR)
 ;

START_T
 : {startToken(start, "{")}? . -> pushMode(VAR)
 ;

UNLESS
 : {startToken(start, "^")}? . -> pushMode(VAR)
 ;

START_BLOCK
 : {startToken(start, "#")}? . -> pushMode(VAR)
 ;

START_DELIM
 : {startToken(start, "=")}? . -> pushMode(SET_DELIMS)
 ;

START_PARTIAL
 : {startToken(start, ">")}? . -> pushMode(PARTIAL)
 ;

END_BLOCK
 : {startToken(start, "/")}? . -> pushMode(VAR)
 ;

START
 : {startToken(start)}? . -> pushMode(VAR)
 ;

SPACE
 :
  [ \t]+
 ;

NL
 :
   '\r'? '\n'
 | '\r'
 ;

mode SET_DELIMS;

END_DELIM
  : {endToken("=" + end)}? . -> popMode
  ;

WS_DELIM
  : [ \t\r\n]
  ;

DELIM: .;

mode PARTIAL;

PATH
  :
  (
    '[' PATH_SEGMENT ']'
  | PATH_SEGMENT
  ) -> mode(VAR)
  ;

fragment
PATH_SEGMENT
  : [a-zA-Z0-9_$'/.:\-]+
  ;
WS_PATH
  : [ \t\r\n] -> skip
  ;
mode VAR;
END_T
 : {endToken(end, "}")}? . -> popMode
 ;
END
 : {endToken(end)}? . -> mode(DEFAULT_MODE)
 ;
DOUBLE_STRING
 :
  '"' ( '\\"' | ~[\n] )*? '"'
 ;
SINGLE_STRING
 :
  '\'' ( '\\\'' | ~[\n] )*? '\''
 ;
EQ
  : '='
  ;
INT
  :
    '-'? [0-9]+
  ;
BOOLEAN
  :
    'true'
  | 'false'
  ;
ELSE
  :
   '~'? 'else' '~'?
  ;
QID
 :
   '../' QID
 | '..'
 | '.'
 | '[' ID ']' ID_SEPARATOR QID
 | '[' ID ']'
 | ID ID_SEPARATOR QID
 | ID
 ;
fragment
ID_SEPARATOR
  : ('.'|'/'|'-');
fragment
ID
  :
  ID_START ID_SUFFIX*
 ;
fragment
ID_START
  :
   [a-zA-Z_$@]
  ;
fragment
ID_SUFFIX
  :
    ID_ESCAPE
  | ID_START
  | ID_PART
  | '-'
  ;
fragment
ID_ESCAPE
  :
   '.' '[' ~[\r\n]+? ']'
  ;
fragment
ID_PART
  :
   [0-9./]
  ;
LP
  :
    '('
  ;
RP
  :
    ')'
  ;
WS
 : [ \t\r\n] -> skip
 ;