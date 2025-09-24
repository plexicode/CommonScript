from .proc import run_process as _run_process
import os as _os

def compile(project_path, target):
    ext = '.exe' if (_os.name == 'nt') else ''
    plexi_tools = _os.environ.get('PLEXI_TOOLS', '../../tools'.replace('/', _os.sep))
    pastel_bin = plexi_tools + _os.sep + 'pastel' + ext
    t = _run_process(pastel_bin, [project_path, target])
    if t.strip() != '':
        bar = '*' * 30
        bar_msg = ' vvv PASTEL ERROR! vvv '
        upper_bar = bar + bar_msg + bar 
        lower_bar = '*' * len(upper_bar)
        print('\n' + upper_bar + '\n')
        print(t.rstrip())
        print('\n' + lower_bar + '\n')

        raise Exception("Pastel compilation error! ^^")
