import type { PluginSimple } from "markdown-it";
import fs from 'fs';

// #include "docs/ja/include.md"
const reg = /#include "(.+)"/;

const markdownItMDinMD: PluginSimple = (md) => {
  const parse = (src) => {
    var cap;
    while(cap = reg.exec(src)) {
      var path = cap[1].trim();
      var txt = parse(fs.readFileSync(path, 'utf8'));
      src = src.substring(0, cap.index) + txt + src.substring(cap.index + cap[0].length);
    }
    return src;
  };

  md.core.ruler.before('normalize', 'markdownItMDinMD', (s) => s.src = parse(s.src));
};

export default markdownItMDinMD;