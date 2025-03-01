'''
Usage:
  You MUST build the pastel code before running this.
  pastel runtime/src/runtime.json javascript|csharp
  python build.py target

  target options:
  - jsruntime: populates all runtime JavaScript for dist/CommonScriptRuntime_*_{ver}.js
  - jscompiler: populates all JavaScript compilers for dist/CommonScriptCompiler_*_{ver}.js
'''

VERSION = (0, 1, 0)
VERSION_DOTTED = '.'.join(map(str, VERSION))
VERSION_UNDERSCORE = VERSION_DOTTED.replace('.', '_')

import os
import sys

def file_read_text(path):
  c = open(path.replace('/', os.sep), 'rt')
  t = c.read().replace('\r\n', '\n')
  c.close()
  return t

def file_write_text(path, content):
  c = open(path.replace('/', os.sep), 'wt', newline='\n')
  c.write(content)
  c.close()

def main(args):
  mode = (args + [''])[:1][0]
  is_js = mode in ('jsruntime', )
  is_js_compiler = mode in ('jscompiler', )

  if is_js:
    dir = 'runtime/templates'
    code = {
      'dist_main': file_read_text(dir + '/dist_template.js'),
      'dist_web': file_read_text(dir + '/dist_template_web.js'),
      'dist_node': file_read_text(dir + '/dist_template_node.js'),
      'dist_plexios': file_read_text(dir + '/dist_template_plexios.js'),
      'gen': file_read_text(dir + '/gen.js'),
      'wrapper': file_read_text(dir + '/commonscript.js'),
    }

    common_script_base_code = (code['dist_main']
      ).replace('%%%VERSION%%%', VERSION_DOTTED
      ).replace('%%%JS_WRAPPER%%%', '\n' + code['wrapper']
      ).replace('%%%PASTEL_GENERATED%%%', '\n' + code['gen'])

    web_code = (code['dist_web']
      ).replace('%%%VERSION%%%', VERSION_DOTTED
      ).replace('%%%COMMON_SCRIPT%%%', common_script_base_code)

    node_code = (code['dist_node']
      ).replace('%%%VERSION%%%', VERSION_DOTTED
      ).replace('%%%COMMON_SCRIPT%%%', common_script_base_code)

    plexios_code = (code['dist_plexios']
      ).replace('%%%VERSION%%%', VERSION_DOTTED
      ).replace('%%%COMMON_SCRIPT%%%', common_script_base_code)

    output_path = 'dist/CommonScriptRuntime_web_' + VERSION_UNDERSCORE + '.js'
    file_write_text(output_path, web_code)
    output_path = 'dist/CommonScriptRuntime_node_' + VERSION_UNDERSCORE + '.js'
    file_write_text(output_path, node_code)
    output_path = 'dist/CommonScriptRuntime_plexios_' + VERSION_UNDERSCORE + '.js'
    file_write_text(output_path, plexios_code)
  elif is_js_compiler:
    dir = 'compiler/templates'
    code = {
      'dist_main': file_read_text(dir + '/dist_template.js'),
      'dist_plexios': file_read_text(dir + '/dist_template_plexios.js'),
      'gen': file_read_text(dir + '/gen.js'),
    }
    common_script_base_code = (code['dist_main']
      ).replace('%%%PASTEL_GENERATED%%%', '\n' + code['gen'])
    plexios_code = (code['dist_plexios']
      ).replace('%%%VERSION%%%', VERSION_DOTTED
      ).replace('%%%VERSION_UNDERSCORE%%%', VERSION_UNDERSCORE
      ).replace('%%%COMMON_SCRIPT%%%', common_script_base_code)

    output_path = 'dist/CommonScriptCompile_plexios_' + VERSION_UNDERSCORE + '.js'
    file_write_text(output_path, plexios_code)
  else:
    return 'ERROR: invalid target option'

  return 'Done'

def copy_builtins():
  items = {}
  for file in os.listdir('builtins'):
    if file.endswith('.script'):
      code = file_read_text(os.path.join('builtins', file))
      # TODO: a real minifier goes here!
      min_lines = []
      for line in code.split('\n'):
        line = line.strip()
        if line[:2] != '//' and line != '':
          min_lines.append(line)

      code = '\n'.join(min_lines)
      code = code.replace('Exception : Exception', '@6')
      code = code.replace('@public function ', '@5')
      code = code.replace('return ', '@4')
      code = code.replace('@public class ', '@3')
      code = code.replace(' { constructor(m=null):base(m){} }', '@2')
      code = code.replace('function ', '@1')
      items[file.split('.')[0]] = code
  keys = list(items.keys())
  keys.sort()
  gen_builtins = ['// This file is generated.', '// Edits should go in builtins/']
  for key in keys:
    code = items[key]
    escaped_code = code.replace('\\', '\\\\').replace('"', '\\"').replace('\r\n', '\n').replace('\n', '\\n')
    gen_builtins.append('string GEN_BUILTINS_' + key + '() { return "' + escaped_code + '"; }')
  file_write_text('compiler/src/builtins/gen_builtins.pst', '\n'.join(gen_builtins) + '\n')
  return 'Done'

if __name__ == '__main__':
  args = sys.argv[1:]
  if len(args) == 1 and args[0] == 'copybuiltins':
    msg = copy_builtins()
  else:
    msg = main(args)
  print(msg)
