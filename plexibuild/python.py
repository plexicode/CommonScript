def strip_imports(code):
    lines = code.split('\n')
    output = []
    for line in lines:
        is_import = line.startswith('import ') or line.startswith('from ')
        if not is_import:
            output.append(line)
    return '\n'.join(output)
