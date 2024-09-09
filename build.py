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
  is_js = mode in ('jsweb', 'jsnode')
  is_node = mode == 'jsnode'

  if not is_js:
    return 'ERROR: invalid target option'

  if is_js:
    dir = 'runtime/templates'
    code = {
      'main': file_read_text(dir + '/dist_template.js'), 
      'gen': file_read_text(dir + '/gen.js'), 
      'wrapper': file_read_text(dir + '/commonscript.js')
    }

    final_code = (code['main']
      ).replace('%%%VERSION%%%', VERSION_DOTTED
      ).replace('%%%JS_WRAPPER%%%', '\n' + code['wrapper']
      ).replace('%%%PASTEL_GENERATED%%%', '\n' + code['gen'])
    
    if is_node:
      return 'ERROR: node not done yet.'

    output_path = 'dist/CommonScriptRuntime_web_' + VERSION_UNDERSCORE + '.js'
    file_write_text(output_path, final_code)

  return 'Done'

if __name__ == '__main__':
  msg = main(sys.argv[1:])
  print(msg)
