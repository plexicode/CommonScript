'''
Usage:
  You MUST build the pastel code before running this.
  pastel runtime/src/runtime.json javascript|csharp
  python build.py target

  target options:
  - jsweb: populates runtime/dist/web/...
  - jsnode: populates runtime/dist/node/...

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
  is_js = mode in ('js', )

  if is_js:
    dir = 'runtime/templates'
    code = {
      'dist_main': file_read_text(dir + '/dist_template.js'),
      'dist_web': file_read_text(dir + '/dist_template_web.js'),
      'dist_node': file_read_text(dir + '/dist_template_node.js'),
      'dist_plexios': file_read_text(dir + '/dist_template_plexios.js'),
      'gen': file_read_text(dir + '/gen.js'),
      'wrapper': file_read_text(dir + '/commonscript.js')
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
  else: 
    return 'ERROR: invalid target option'

  return 'Done'

if __name__ == '__main__':
  msg = main(sys.argv[1:])
  print(msg)
