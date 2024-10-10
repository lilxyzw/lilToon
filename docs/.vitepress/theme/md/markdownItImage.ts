import type { PluginSimple } from "markdown-it";

const markdownItImage: PluginSimple = (md) => {
  const defaultRender =
    md.renderer.rules.image ||
      function (tokens, idx, options, env, self) {
      return self.renderToken(tokens, idx, options);
    };

  md.renderer.rules.image = function (tokens, idx, options, env, self) {
    const token = tokens[idx];
    if(token.attrs != null)
    {
      var result = '<a href=\"/lilToon';
      result += token.attrs[token.attrIndex('src')][1];
      result += '\"data-lightbox=' + crypto.randomUUID() + '>';
      result += defaultRender(tokens, idx, options, env, self);
      result += '</a>';
      return result;
    }
    return defaultRender(tokens, idx, options, env, self);
  }
};

export default markdownItImage;