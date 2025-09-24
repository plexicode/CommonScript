from .proc import run_process as _run_process

def strip_comments(code):
    lines = code.split('\n')
    output = []
    for original_line in lines:
        line = original_line.strip()
        if line == '': continue
        if line.startswith('//'): continue
        output.append(original_line)
    return '\n'.join(output) + '\n'

# Deprecate this.
def simple_minify(code):
    code = strip_comments(code).strip()
    lines = list(map(lambda line: line.strip(), code.split('\n')))
    output = [lines[0]]
    for line in lines[1:]:
        append_to_last = False
        if len(output) > 0:
            first_char = line[0]
            prev_char = output[-1][-1]
            if prev_char in ';{([,/+-.<>': append_to_last = True
            if first_char in ';})],.<>': append_to_last = True
            if len(output[-1]) > 300: append_to_last = False

        if not append_to_last:
            output.append('\n')
        output.append(line)
    output.append('\n')
    return ''.join(output)

def minify(path):
    output = _run_process('terser', [path, '--compress', '--mangle'])
    return output
