import os as _os

def _ensure_escaped_string(value):
    value = str(value)
    for c in ' "\'':
        if c in value:
            return '"' + value.replace('"', '\\"') + '"'
    return value

def run_process(ex, args = None, cwd = None):
    if cwd == None:
        return run_process_impl(ex, args)

    original_cwd = _os.path.abspath(_os.curdir)
    try:
        _os.chdir(cwd)
        return run_process_impl(ex, args)
    finally:
        _os.chdir(original_cwd)

def run_process_impl(ex, args = None):
    cmd = [ex]
    if args != None:
        if type(args) == type(''):
            cmd.append(args)
        else:
            for arg in args:
                cmd.append(arg)

    cmd = (ex + ' ' + ' '.join(map(_ensure_escaped_string, args))).strip()

    c = _os.popen(cmd)
    t = c.read()
    c.close()
    return t
