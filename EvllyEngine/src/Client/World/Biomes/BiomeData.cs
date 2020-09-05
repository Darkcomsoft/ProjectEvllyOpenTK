﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.World.Biomes
{
    public struct BiomeData
    {
        public TypeBlock _typeBlock;
        public BlockVariant _blockVariant;
        public TreeType _treeType;

        public BiomeData(TypeBlock typeBlock, BlockVariant blockVariant, TreeType treeType)
        {
            _typeBlock = typeBlock;
            _blockVariant = blockVariant;
            _treeType = treeType;
        }
    }
}
