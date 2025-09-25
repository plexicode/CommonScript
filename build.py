
import plexibuild
from plexibuild import fileio
from plexibuild import pastel
from plexibuild import textpreprocessor
from plexibuild import javascript

import json

VERSION_DOTTED = fileio.file_read_text("./current-version.txt").strip().split('\n').pop()
VERSION_UNDERSCORE = VERSION_DOTTED.replace('.', '_')
VERSION = tuple(map(lambda s: int(s), VERSION_DOTTED.split('.')))

def build_js_runtime():
  dir = './runtime/templates'
  code = {
    'dist_main': fileio.file_read_text(dir + '/dist_template.js'),
    'dist_web': fileio.file_read_text(dir + '/dist_template_web.js'),
    'dist_node': fileio.file_read_text(dir + '/dist_template_node.js'),
    'dist_plexios': fileio.file_read_text(dir + '/dist_template_plexios.js'),
    'gen': fileio.file_read_text(dir + '/gen.js'),
    'wrapper': fileio.file_read_text(dir + '/commonscript.js'),
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

  files = {}
  files['CommonScriptRuntime_web_' + VERSION_UNDERSCORE + '.js'] = web_code
  files['CommonScriptRuntime_node_' + VERSION_UNDERSCORE + '.js'] = node_code
  files['CommonScriptRuntime_plexios_' + VERSION_UNDERSCORE + '.js'] = plexios_code
  fileio.ensure_directory('./dist')
  fileio.batch_write_to_dir('./dist', files)

def build_js_compiler():
  dir = 'compiler/templates'
  code = {
    'dist_main': fileio.file_read_text(dir + '/dist_template.js'),
    'dist_plexios': fileio.file_read_text(dir + '/dist_template_plexios.js'),
    'gen': fileio.file_read_text(dir + '/gen.js'),
  }
  common_script_base_code = (code['dist_main']
    ).replace('%%%PASTEL_GENERATED%%%', '\n' + code['gen'])
  plexios_code = (code['dist_plexios']
    ).replace('%%%VERSION%%%', VERSION_DOTTED
    ).replace('%%%VERSION_UNDERSCORE%%%', VERSION_UNDERSCORE
    ).replace('%%%COMMON_SCRIPT%%%', common_script_base_code)

  output_path = 'dist/CommonScriptCompile_plexios_' + VERSION_UNDERSCORE + '.js'
  fileio.ensure_directory('./dist')
  fileio.file_write_text(output_path, plexios_code)

def copy_builtins():
  items = {}
  for file in fileio.list_files('./builtins'):
    if file.endswith('.script'):
      code = fileio.file_read_text('./builtins/' + file)
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
  fileio.file_write_text('./compiler/src/builtins/gen_builtins.pst', '\n'.join(gen_builtins) + '\n')

def get_test_cases_as_json():
  all_files_in_tests = fileio.list_files_recursive('./tests')
  config_files = filter(lambda t: t.endswith('.config') and len(t.split('/')) == 2, all_files_in_tests)
  test_dirs = list(map(lambda t: t.split('/')[0], config_files))
  copy_dirs = set(list(test_dirs) + ['testlib'])

  output = []
  for path in all_files_in_tests:
    if path.split('/')[0] in copy_dirs:
      output.append({ 'path': path, 'content': fileio.file_read_text('./tests/' + path)})
  return { 'files': output, 'testCases': test_dirs }

def main():
  fileio.ensure_directory('./dist')

  copy_builtins()

  pastel.compile('./compiler/src/compiler.json', 'javascript')
  pastel.compile('./runtime/src/runtime.json', 'javascript')

  js_ver_file = 'const VER = [' + VERSION_DOTTED.replace('.', ', ') + '];\n'
  fileio.file_write_text('./compiler/src-intermediate/GEN-version.js', js_ver_file)
  fileio.file_write_text('./runtime/src-intermediate/GEN-version.js', js_ver_file)

  for is_debug in (True, False):
    debug_vars = { 'IS_DEBUG': is_debug }
    suffix = '.debug.js' if is_debug else '.js'
    fileio.file_write_text('./dist/compiler-lib' + suffix, textpreprocessor.load_javascript_file('./compiler/src-intermediate/lib.js', debug_vars))
    fileio.file_write_text('./dist/runtime-lib' + suffix, textpreprocessor.load_javascript_file('./runtime/src-intermediate/lib.js', debug_vars))

  vars_for_node = {}
  # vars_for_node['IS_DEBUG'] = True

  fileio.batch_write_to_dir('./dist', {
    'compiler-lib.min.js': javascript.minify('./dist/compiler-lib.js'),
    'runtime-lib.min.js': javascript.minify('./dist/runtime-lib.js'),
    'compiler-lib.node.js': textpreprocessor.load_javascript_file('./compiler/src-intermediate/lib-node.js', vars_for_node),
    'runtime-lib.node.js': textpreprocessor.load_javascript_file('./runtime/src-intermediate/lib-node.js', vars_for_node),
  })

  fileio.copy_file('./dist/compiler-lib.node.js', './tests/harnesses/node/testscript/GEN-compiler.js')
  fileio.copy_file('./dist/runtime-lib.node.js', './tests/harnesses/node/testscript/GEN-runtime.js')

  fileio.file_write_text(
    './tests/harnesses/node/testdata/GEN-cases.js',
    'let data = ' + json.dumps(json.dumps(get_test_cases_as_json())) + ';\n' +
    'export function getTestData() { return JSON.parse(data); };\n')

  # build_js_compiler()
  # build_js_runtime()

  plexibuild.display_completion_message()

if __name__ == '__main__':
  main()
