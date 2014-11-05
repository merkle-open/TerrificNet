grammar Mustache;


/*
 * Parser Rules
 */

mustache : content;

content:   (block)+;

block : OPEN expression CLOSE STRING? OPEN BLOCKCLOSING Name CLOSE;
expression: (VARIABLEEXPESSION Name);

compileUnit
	:	EOF
	;

/*
 * Lexer Rules
 */
 

Name: NameStartChar NameChar*;

OPEN: '{{';
CLOSE:'}}';
BLOCKCLOSING: '/';
VARIABLEEXPESSION : '#';

STRING :  (~["\\])*;

WS
	:	' ' -> channel(HIDDEN)
	;

fragment
DIGIT       :   [0-9] ;

fragment
NameChar    :   NameStartChar
            |   '-' | '_' | '.' | DIGIT 
            |   '\u00B7'
            |   '\u0300'..'\u036F'
            |   '\u203F'..'\u2040'
            ;

fragment
NameStartChar
            :   [:a-zA-Z]
            |   '\u2070'..'\u218F' 
            |   '\u2C00'..'\u2FEF' 
            |   '\u3001'..'\uD7FF' 
            |   '\uF900'..'\uFDCF' 
            |   '\uFDF0'..'\uFFFD'
            ;
