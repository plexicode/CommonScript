from .proc import run_process as _run_process
import json as _json

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

def minify_plus(path):
    original_code = minify(path)

    two_byte_chars = 'À Á Â Ã Ä Å Æ Ç È Ê Ë Ì Í Î Ï Ð Ñ Ò Ó Ô Õ Ö Ø Ù Ú Û'.split(' ')[::-1]
    two_byte_chars = list(filter(lambda c: c not in original_code, two_byte_chars))
    sequences = [
        '\\', # Some issues with escape sequences being swapped back in
        '`', # Whole final string is wrapped in backticks.
        '${', # Prevent accidental templates

        '=function(',
        'function(',
        'Object.keys',
        'null',
        'return',
        'continue',
        'break',
        'switch',
        'case',
        '.length',
        '.join("',
        'let ',
        ';for(',
        '][0]',
        'new ',
        'void 0!==',
        '.push(',
        'throw new Error(',
        ';if(',
        'Exception',
    ]

    if len(sequences) > len(two_byte_chars): raise Exception()

    swaps = []
    code = original_code
    lookup = {}
    for seq in sequences:
        if '#' in seq: raise Exception() # used as a delimiter later
        if seq in code:
            c = two_byte_chars.pop()
            swaps.append((c, seq))
            lookup[c] = seq
            code = code.replace(seq, c)

    final_code = code
    roundtrip_verify = []

    for c in final_code:
        roundtrip_verify.append(lookup.get(c, c))
    roundtrip_verify_flat = ''.join(roundtrip_verify)

    if roundtrip_verify_flat != original_code:
        raise Exception()

    swap_lookup = []
    for swap in swaps:
        swap_lookup.append(swap[0])
        swap_lookup.append(swap[1])

    return ''.join([
        'eval((c => {',
            'let s=' + _json.dumps('#'.join(swap_lookup)), '.split("#");',
            'let w={};',
            'for(let i=0; i<s.length; i+=2){',
                'w[s[i]]=s[i+1];',
            '}',
            'let b="";',
            'for(let h of c.split("")) {',
                'b+=w[h]||h;'
            '}',

            'return b;',
        '})(`', final_code, '`));\n'
    ])
