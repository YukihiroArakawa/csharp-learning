{
  description = "C# 学習 5: 最終課題 API";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs = { nixpkgs, flake-utils, ... }:
    flake-utils.lib.eachDefaultSystem (system:
      let pkgs = import nixpkgs { inherit system; };
      in {
        devShells.default = pkgs.mkShell {
          packages = with pkgs; [ dotnet-sdk_10 csharp-ls netcoredbg sqlite ];
          LD_LIBRARY_PATH = pkgs.lib.makeLibraryPath [ pkgs.sqlite ];
        };
      });
}
