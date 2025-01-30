#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

using Object = UnityEngine.Object;

namespace lilToon
{
    public partial class lilToonInspector
    {
        //------------------------------------------------------------------------------------------------------------------------------
        // Shader variables
        protected static Shader lts         { get { return lilShaderManager.lts       ; } set { lilShaderManager.lts        = value; } }
        protected static Shader ltsc        { get { return lilShaderManager.ltsc      ; } set { lilShaderManager.ltsc       = value; } }
        protected static Shader ltst        { get { return lilShaderManager.ltst      ; } set { lilShaderManager.ltst       = value; } }
        protected static Shader ltsot       { get { return lilShaderManager.ltsot     ; } set { lilShaderManager.ltsot      = value; } }
        protected static Shader ltstt       { get { return lilShaderManager.ltstt     ; } set { lilShaderManager.ltstt      = value; } }
        protected static Shader ltso        { get { return lilShaderManager.ltso      ; } set { lilShaderManager.ltso       = value; } }
        protected static Shader ltsco       { get { return lilShaderManager.ltsco     ; } set { lilShaderManager.ltsco      = value; } }
        protected static Shader ltsto       { get { return lilShaderManager.ltsto     ; } set { lilShaderManager.ltsto      = value; } }
        protected static Shader ltsoto      { get { return lilShaderManager.ltsoto    ; } set { lilShaderManager.ltsoto     = value; } }
        protected static Shader ltstto      { get { return lilShaderManager.ltstto    ; } set { lilShaderManager.ltstto     = value; } }
        protected static Shader ltsoo       { get { return lilShaderManager.ltsoo     ; } set { lilShaderManager.ltsoo      = value; } }
        protected static Shader ltscoo      { get { return lilShaderManager.ltscoo    ; } set { lilShaderManager.ltscoo     = value; } }
        protected static Shader ltstoo      { get { return lilShaderManager.ltstoo    ; } set { lilShaderManager.ltstoo     = value; } }
        protected static Shader ltstess     { get { return lilShaderManager.ltstess   ; } set { lilShaderManager.ltstess    = value; } }
        protected static Shader ltstessc    { get { return lilShaderManager.ltstessc  ; } set { lilShaderManager.ltstessc   = value; } }
        protected static Shader ltstesst    { get { return lilShaderManager.ltstesst  ; } set { lilShaderManager.ltstesst   = value; } }
        protected static Shader ltstessot   { get { return lilShaderManager.ltstessot ; } set { lilShaderManager.ltstessot  = value; } }
        protected static Shader ltstesstt   { get { return lilShaderManager.ltstesstt ; } set { lilShaderManager.ltstesstt  = value; } }
        protected static Shader ltstesso    { get { return lilShaderManager.ltstesso  ; } set { lilShaderManager.ltstesso   = value; } }
        protected static Shader ltstessco   { get { return lilShaderManager.ltstessco ; } set { lilShaderManager.ltstessco  = value; } }
        protected static Shader ltstessto   { get { return lilShaderManager.ltstessto ; } set { lilShaderManager.ltstessto  = value; } }
        protected static Shader ltstessoto  { get { return lilShaderManager.ltstessoto; } set { lilShaderManager.ltstessoto = value; } }
        protected static Shader ltstesstto  { get { return lilShaderManager.ltstesstto; } set { lilShaderManager.ltstesstto = value; } }
        protected static Shader ltsl        { get { return lilShaderManager.ltsl      ; } set { lilShaderManager.ltsl       = value; } }
        protected static Shader ltslc       { get { return lilShaderManager.ltslc     ; } set { lilShaderManager.ltslc      = value; } }
        protected static Shader ltslt       { get { return lilShaderManager.ltslt     ; } set { lilShaderManager.ltslt      = value; } }
        protected static Shader ltslot      { get { return lilShaderManager.ltslot    ; } set { lilShaderManager.ltslot     = value; } }
        protected static Shader ltsltt      { get { return lilShaderManager.ltsltt    ; } set { lilShaderManager.ltsltt     = value; } }
        protected static Shader ltslo       { get { return lilShaderManager.ltslo     ; } set { lilShaderManager.ltslo      = value; } }
        protected static Shader ltslco      { get { return lilShaderManager.ltslco    ; } set { lilShaderManager.ltslco     = value; } }
        protected static Shader ltslto      { get { return lilShaderManager.ltslto    ; } set { lilShaderManager.ltslto     = value; } }
        protected static Shader ltsloto     { get { return lilShaderManager.ltsloto   ; } set { lilShaderManager.ltsloto    = value; } }
        protected static Shader ltsltto     { get { return lilShaderManager.ltsltto   ; } set { lilShaderManager.ltsltto    = value; } }
        protected static Shader ltsref      { get { return lilShaderManager.ltsref    ; } set { lilShaderManager.ltsref     = value; } }
        protected static Shader ltsrefb     { get { return lilShaderManager.ltsrefb   ; } set { lilShaderManager.ltsrefb    = value; } }
        protected static Shader ltsfur      { get { return lilShaderManager.ltsfur    ; } set { lilShaderManager.ltsfur     = value; } }
        protected static Shader ltsfurc     { get { return lilShaderManager.ltsfurc   ; } set { lilShaderManager.ltsfurc    = value; } }
        protected static Shader ltsfurtwo   { get { return lilShaderManager.ltsfurtwo ; } set { lilShaderManager.ltsfurtwo  = value; } }
        protected static Shader ltsfuro     { get { return lilShaderManager.ltsfuro   ; } set { lilShaderManager.ltsfuro    = value; } }
        protected static Shader ltsfuroc    { get { return lilShaderManager.ltsfuroc  ; } set { lilShaderManager.ltsfuroc   = value; } }
        protected static Shader ltsfurotwo  { get { return lilShaderManager.ltsfurotwo; } set { lilShaderManager.ltsfurotwo = value; } }
        protected static Shader ltsgem      { get { return lilShaderManager.ltsgem    ; } set { lilShaderManager.ltsgem     = value; } }
        protected static Shader ltsfs       { get { return lilShaderManager.ltsfs     ; } set { lilShaderManager.ltsfs      = value; } }
        protected static Shader ltsover     { get { return lilShaderManager.ltsover   ; } set { lilShaderManager.ltsover    = value; } }
        protected static Shader ltsoover    { get { return lilShaderManager.ltsoover  ; } set { lilShaderManager.ltsoover   = value; } }
        protected static Shader ltslover    { get { return lilShaderManager.ltslover  ; } set { lilShaderManager.ltslover   = value; } }
        protected static Shader ltsloover   { get { return lilShaderManager.ltsloover ; } set { lilShaderManager.ltsloover  = value; } }
        protected static Shader ltsbaker    { get { return lilShaderManager.ltsbaker  ; } set { lilShaderManager.ltsbaker   = value; } }
        protected static Shader ltspo       { get { return lilShaderManager.ltspo     ; } set { lilShaderManager.ltspo      = value; } }
        protected static Shader ltspc       { get { return lilShaderManager.ltspc     ; } set { lilShaderManager.ltspc      = value; } }
        protected static Shader ltspt       { get { return lilShaderManager.ltspt     ; } set { lilShaderManager.ltspt      = value; } }
        protected static Shader ltsptesso   { get { return lilShaderManager.ltsptesso ; } set { lilShaderManager.ltsptesso  = value; } }
        protected static Shader ltsptessc   { get { return lilShaderManager.ltsptessc ; } set { lilShaderManager.ltsptessc  = value; } }
        protected static Shader ltsptesst   { get { return lilShaderManager.ltsptesst ; } set { lilShaderManager.ltsptesst  = value; } }
        protected static Shader ltsm        { get { return lilShaderManager.ltsm      ; } set { lilShaderManager.ltsm       = value; } }
        protected static Shader ltsmo       { get { return lilShaderManager.ltsmo     ; } set { lilShaderManager.ltsmo      = value; } }
        protected static Shader ltsmref     { get { return lilShaderManager.ltsmref   ; } set { lilShaderManager.ltsmref    = value; } }
        protected static Shader ltsmfur     { get { return lilShaderManager.ltsmfur   ; } set { lilShaderManager.ltsmfur    = value; } }
        protected static Shader ltsmgem     { get { return lilShaderManager.ltsmgem   ; } set { lilShaderManager.ltsmgem    = value; } }
        protected static Shader mtoon       { get { return lilShaderManager.mtoon     ; } set { lilShaderManager.mtoon      = value; } }
    }
}
#endif
