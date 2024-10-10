import type { PluginSimple } from "markdown-it";

const markdownItToc: PluginSimple = (md) => {
  const defaultRender =
    md.renderer.rules.toc_body ||
      function (tokens, idx, options, env, self) {
      return self.renderToken(tokens, idx, options);
    };

  md.renderer.rules.toc_body = function (tokens, idx, options, env, self) {
    var result = '<h1>Index</h1>';
    result += defaultRender(tokens, idx, options, env, self);
    return result;
  }
};

export default markdownItToc;