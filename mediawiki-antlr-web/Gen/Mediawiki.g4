grammar Mediawiki;


document: line (NL line?)* EOF;

line: horizontal_line | list | heading | paragraph | image | table;

/* formatting */
formatted: italic | bold | bold_italic;
unformatted: (link | plaintext)+;

italic: ITALIC unformatted ITALIC;
bold: BOLD unformatted BOLD;
bold_italic: BOLD_ITALIC unformatted BOLD_ITALIC;

/* headings */
heading: heading1 | heading2 | heading3 | heading4 | heading5 | heading6;
heading1: H1 plaintext H1;
heading2: H2 plaintext H2;
heading3: H3 plaintext H3;
heading4: H4 plaintext H4;
heading5: H5 plaintext H5;
heading6: H6 plaintext H6;

/* horizontal line */
horizontal_line: HRLINE;

/* lists */
list: unordered_list | ordered_list;

unordered_list: (unordered_list_item NL)* unordered_list_item;
ordered_list: (ordered_list_item NL)* ordered_list_item;

unordered_list_item: '*' list_item_content;
ordered_list_item: '#' list_item_content;

list_item_content: unordered_list_item | ordered_list_item | content;

/* paragraph */
paragraph: content;

content: (formatted | unformatted)+;

/* links */
link: external_link | internal_link;

external_link: ('['? external_link_uri ']'?) | ('[' external_link_uri (WHITESPACE external_link_title)? ']');
internal_link: '[[' internal_link_ref ('|' internal_link_title)? ']]';

external_link_uri: ('http' | 'https') '://' (CHARACTER+ '.'* '='*)+;
external_link_title: plaintext;
internal_link_ref: plaintext;
internal_link_title: plaintext;

/* image */
image: '[[File:' image_filename ('|' image_caption)? ']]';

image_filename: plaintext '.' plaintext;
image_caption: plaintext;

/* table */
table: '{|' NL (table_row_separator NL)? (table_row NL table_row_separator NL)* table_row NL '|}';

table_row_separator: '|-';
table_row: ('|' NL? table_cell ('||' table_cell)*) | ('|' NL? table_cell (NL '|' NL? table_cell)*);
table_cell: line;

/* plaintext */
plaintext: (CHARACTER | WHITESPACE)+;


/**
 * LEXER RULES
 * --------------------------------------------------------------------------
 */

ITALIC: '\'\'';
BOLD: '\'\'\'';
BOLD_ITALIC: '\'\'\'\'\'';

H1: '=';
H2: '==';
H3: '===';
H4: '====';
H5: '=====';
H6: '======';

HRLINE: '---' '-'+;

CHARACTER       :       '!' | '"' | '#' | '$' | '%' | '&'
                |       '*' | '+' | ',' | '-' | '.' | '/'
                |       ':' | ';' | '?' | '@' | '\\' | '^' | '_' | '`' | '~'
                |       '0'..'9' | 'A'..'Z' |'a'..'z' 
                |       '\u0080'..'\u7fff'
                |       '(' | ')'
                |       '\'' | '<' | '>' | '=' | '[' | ']' | '|' 
                ;

WHITESPACE: ' ' | '\t';

NL: '\r'? '\n';