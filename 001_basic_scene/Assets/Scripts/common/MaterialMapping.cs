using UnityEngine;
using System;

[Serializable]
public class MaterialMapping{
    public enum MaterialMappingEnum{
        blue,
        grey_blue,
        grid_blue,
        grid_orange,
        grid_white,
        transparent,
        mirror,
		fake_grid_orange
    }

    public Material blue;
    public Material grey_blue;
    public Material grid_blue;
    public Material grid_orange;
    public Material fake_grid_orange;
    public Material grid_white;
    public Material transparent;
    public Material mirror;
    public bool isMonteCarlo;

    public Material GetMaterial(MaterialMappingEnum materialType){
        switch(materialType){
            case MaterialMappingEnum.blue:{
                return blue;
            }
            case MaterialMappingEnum.grey_blue:{
                return grey_blue;
            }
            case MaterialMappingEnum.grid_blue:{
                return grid_blue;
            }
            case MaterialMappingEnum.grid_orange:{
                return grid_orange;
            }
            case MaterialMappingEnum.fake_grid_orange:{
                return fake_grid_orange;
            }
            case MaterialMappingEnum.grid_white:{
                return grid_white;
            }
            case MaterialMappingEnum.transparent:{
                return transparent;
            }
            case MaterialMappingEnum.mirror:{
                return mirror;
            }
        }
        return null;
    }
}