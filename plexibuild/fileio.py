import os as _os
import shutil as _shutil
import json as _json

def _to_real_path(path):
    return path.replace('/', _os.sep)

def to_canonical_path(path):
    parts = path.replace('\\', '/').split('/')
    output = []
    for part in parts:
        if part == '': output = []
        elif part == '.': pass
        elif part == '..':
            if len(output) == 0 or output[-1] == '..':
                output.append('..')
            else:
                output.pop()
        else:
            output.append(part)
    return '/'.join(output)

def get_parent(path):
    return to_canonical_path(path + '/..')

def path_exists(path):
    return _os.path.exists(_to_real_path(to_canonical_path(path)))

def file_read_text(path):
    return file_read_bytes(path).decode('utf-8').replace('\r\n', '\n')

def file_read_json(path):
    return _json.loads(file_read_text(path))

def file_read_bytes(path):
    c = open(_to_real_path(to_canonical_path(path)), 'rb')
    byte_arr = c.read()
    c.close()
    return byte_arr

def file_write_text(path, txt):
    c = open(_to_real_path(to_canonical_path(path)), 'wb')
    c.write(txt.encode('utf-8'))
    c.close()

def batch_write_to_dir(dir, files):
    for rel_path in files.keys():
        content = files[rel_path]
        if type(content) == str:
            file_write_text(dir + '/' + rel_path, content)
        # TODO: if byte array
        else:
            raise Exception()

def ensure_directory(path):
    real_path = _to_real_path(to_canonical_path(path))
    if _os.path.exists(real_path): return
    _os.makedirs(real_path)

def copy_file(src_path, dst_path):
    src_path = to_canonical_path(src_path)
    dst_path = to_canonical_path(dst_path)
    _shutil.copyfile(_to_real_path(src_path), _to_real_path(dst_path))

def copy_directory_contents_to(src_path, dst_path):
    files = list_files_recursive(src_path)
    all_dirs = {}
    for file in files:
        dir = get_parent(file)
        if dir in ('', '.', '..'): continue
        all_dirs[dir] = dir
    for dir in all_dirs.values():
        ensure_directory(dst_path + '/' + dir)

    for file in files:
        copy_file(src_path + '/' + file, dst_path + '/' + file)


def _list_dir(dir, get_files):
    output = []
    for file in _os.listdir(_to_real_path(to_canonical_path(dir))):
        if _os.path.isdir(dir + _os.sep + file) != get_files:
            output.append(file)
    return output

def list_files(dir): return _list_dir(dir, True)
def list_directories(dir): return _list_dir(dir, False)

def _list_files_recursive_impl(rel, abs, out):
    for file in _os.listdir(abs):
        file_rel = file if rel == '' else (rel + '/' + file)
        file_abs = abs + _os.sep + file
        if _os.path.isdir(file_abs):
            _list_files_recursive_impl(file_rel, file_abs, out)
        else:
            out.append(file_rel)

def list_files_recursive(dir):
    out = []
    _list_files_recursive_impl('', _to_real_path(to_canonical_path(dir)), out)
    return out

__all__ = [
    'copy_directory_contents_to',
    'copy_file',
    'batch_write_to_dir',
    'ensure_directory',
    'file_read_bytes',
    'file_read_json',
    'file_read_text',
    'file_write_text',
    'get_parent',
    'list_directories',
    'list_files',
    'list_files_recursive',
    'path_exists',
    'to_canonical_path',
]
