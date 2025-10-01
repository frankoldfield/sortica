source "https://rubygems.org"

# Jekyll version - ahora TÚ controlas la versión (última versión: 4.4.1)
gem "jekyll", "~> 4.4.1"


# Plugins - versiones más recientes
group :jekyll_plugins do
  gem "jekyll-feed", "~> 0.17"
  gem "jekyll-include-cache", "~> 0.2"
  gem "jekyll-spaceship", "~> 0.10.2"
  gem "jekyll-pdf-embed", "~> 1.1.2"
  gem "jekyll-sitemap", "~> 1.4"
  gem "jekyll-seo-tag", "~> 2.8"
  gem "jekyll-paginate", "~> 1.1"
  gem "jekyll-gist", "~> 1.5"
end

# Windows and JRuby does not include zoneinfo files
platforms :mingw, :x64_mingw, :mswin, :jruby do
  gem "tzinfo", ">= 1", "< 3"
  gem "tzinfo-data"
end

# Performance-booster for watching directories on Windows
gem "wdm", "~> 0.1", platforms: [:mingw, :x64_mingw, :mswin]

# Lock `http_parser.rb` gem to `v0.6.x` on JRuby builds
gem "http_parser.rb", "~> 0.6.0", platforms: [:jruby]

# Webrick is needed for Jekyll 4+ on Ruby 3+
gem "webrick", "~> 1.8"