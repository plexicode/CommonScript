from .fileio import file_read_text as _file_read_text
from .fileio import to_canonical_path as _to_canonical_path

'''
    Loads a text file with C-style pre-processing directives
    (conditions and paths in {}'s do not have {} around them in the real version)

    Conditionals...

    #IF {variable}
    #ELSE
    #ENDIF

    Recurisvely includes the given file.
    #INCLUDE {relative path}

    Drops in a base64 string of the given file (double-quoted)
    #BASE64 {relative path to binary file}

    #COMMENT a super-secret comment that gets removed from the output even if not minified.
'''
def load_text_file(path, vars, line_comment_prefix = None):
    if vars == None: vars = {}
    sb = []
    doc = parse_document(path, _file_read_text(path), line_comment_prefix)
    _flatten_action(path, doc, vars, sb, line_comment_prefix)
    return '\n'.join(sb)

def _trim_line_ends(s):
    return '\n'.join(map(lambda v: v.rstrip(), s.split('\n')))

def load_javascript_file(path, vars = None):
    return load_curly_brace_file(path, vars)

def load_curly_brace_file(path, vars = None):
    return _trim_line_ends(load_text_file(path, vars, '//'))

def load_python_file(path, vars = None):
    return _trim_line_ends(load_text_file(path, vars, '#'))

def load_html_file(path, vars = None):
    return _trim_line_ends(load_text_file(path, vars, '<@>'))

def _parse_directive(line, comment):
    actual_line = line.strip()
    if actual_line == '': return None
    if comment != None and actual_line.startswith(comment):
        actual_line = actual_line[len(comment):].strip()

    if actual_line[0] != '#': return None
    parts = actual_line[1:].split(' ')
    if parts[0].strip().upper() != parts[0] or parts[0] == '': return None

    instr = parts[0]
    args = parts[1:]
    argc = len(parts) - 1
    if instr == 'IF':
        if argc != 1: raise Exception()
        return { 'action': 'IF', 'variable': parts[1] }

    if instr == 'ELSEIF' or instr == 'ELIF':
        if argc != 1: raise Exception()
        return { 'action': 'ELSEIF', 'variable': parts[1] }

    if instr == 'ELSE':
        if argc != 0: raise Exception()
        return { 'action': 'ELSE' }

    if instr == 'ENDIF':
        if argc != 0: raise Exception()
        return { 'action': 'ENDIF' }

    if instr == 'INCLUDE' or instr == 'INCLUDE-INDENT':
        if argc != 1: raise Exception()
        return {
            'action': 'INCLUDE',
            'path': args[0],
            'indent': '' if instr == 'INCLUDE' else ' ' * (len(line) - len(line.lstrip()))
        }

    if instr == 'BASE64':
        if argc != 1: raise Exception()
        raise Exception("Not implemented yet.")

    if instr == 'COMMENT':
        return { 'action': 'NOOP' }

    raise Exception("Unrecognized text preprocessor directive: #" + instr)

class _LineStream:
    def __init__(self, path, text, comment_token):

        self.path = path
        self.lines = []
        for line in text.split('\n'):
            directive = _parse_directive(line, comment_token)
            if directive != None:
                self.lines.append(directive)
            else:
                self.lines.append({ 'action': 'TEXT', 'value': line })
        self.index = 0
        self.length = len(self.lines)

    def next_action(self):
        if self.index < self.length: return self.lines[self.index]['action']
        return 'EOF'
    def pop(self):
        line = self.lines[self.index]
        self.index += 1
        return line
    def peek(self):
        if self.index < self.length: return self.lines[self.index]
        return { 'action': 'EOF' }
    def has_more(self):
        return self.index < self.length

def parse_document(path, text, comment_token):
    tokens = _LineStream(path, text, comment_token)
    commands = parse_command_block(path, tokens, comment_token)
    if tokens.has_more():
        raise Exception("Unexpected directive: " + tokens.next_action())
    return commands

def parse_command_block(current_path, tokens, comment_token):
    output = []
    while tokens.has_more():
        action = tokens.next_action()
        if action == 'TEXT':
            output.append(tokens.pop())
        elif action == 'IF':
            output.append(parse_if(current_path, tokens, comment_token))
        elif action in ('INCLUDE', 'BASE64', 'NOOP'):
            token = tokens.pop()
            output.append(token)
        else:
            break

    return { 'action': 'BLOCK', 'commands': output }

def parse_if(current_path, tokens, comment_token):
    next = tokens.next_action()
    if next not in ('IF', 'ELSEIF'): raise Exception()
    if_token = tokens.pop()
    block = parse_command_block(current_path, tokens, comment_token)
    next = tokens.next_action()
    if next == 'ELSEIF':
        else_block = parse_if(current_path, tokens, comment_token)
    elif next == 'ELSE':
        tokens.pop()
        else_block = parse_command_block(current_path, tokens, comment_token)
        if tokens.pop()['action'] != 'ENDIF': raise Exception()
    elif next == 'ENDIF':
        else_block = { 'action': 'NOOP' }
        tokens.pop()

    return { 'action': 'IF', 'variable': if_token['variable'], 'ifCode': block, 'elseCode': else_block }

def _flatten_action(path, command, vars, sb, comment_token, indent = ''):
    action_id = command['action']
    if action_id == 'TEXT':
        sb.append((indent + command['value']).rstrip())
    elif action_id == 'BLOCK':
        for line in command['commands']:
            _flatten_action(path, line, vars, sb, comment_token, indent)
    elif action_id == 'IF':
        block = command['ifCode' if vars.get(command['variable'], False) else 'elseCode']
        _flatten_action(path, block, vars, sb, comment_token, indent)
    elif action_id == 'INCLUDE':
        new_path = _to_canonical_path(path + '/../' + command['path'])
        new_doc = parse_document(new_path, _file_read_text(new_path), comment_token)
        _flatten_action(new_path, new_doc, vars, sb, comment_token, indent + command['indent'])
    elif action_id == 'BASE64':
        raise Exception("Not implemented")
    elif action_id == 'NOOP':
        pass
    else:
        raise Exception("Unknown directive after parse: " + action_id)
