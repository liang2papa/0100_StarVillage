using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class ItemDataImporter : EditorWindow
{
    // ... (기존 경로 변수들은 동일) ...
    private string m_generalCSVPath = "Assets/00_StarVillage/Data/CSV/GeneralItem.csv";
    private string m_equipmentCSVPath = "Assets/00_StarVillage/Data/CSV/Equipment.csv";

    private string m_savePathBase = "Assets/00_StarVillage/Data/ScriptableObjects/Entities/ItemEntity";

    // [설정] 어드레서블 그룹 이름
    private const string ADDRESSABLE_GROUP_NAME = "ItemData";

    [MenuItem("Tools/StarVillage/Import Item Data (Addressable)")]
    public static void ShowWindow()
    {
        GetWindow<ItemDataImporter>("Item Importer");
    }
    private void OnGUI()
    {
        GUILayout.Label("CSV to ScriptableObject Importer", EditorStyles.boldLabel);

        GUILayout.Space(10);
        GUILayout.Label("CSV File Paths (Relative to Project)", EditorStyles.label);
        m_generalCSVPath = EditorGUILayout.TextField("General CSV:", m_generalCSVPath);
        m_equipmentCSVPath = EditorGUILayout.TextField("Equipment CSV:", m_equipmentCSVPath);

        GUILayout.Space(10);
        if (GUILayout.Button("Import All Items"))
        {
            ImportGeneralItems();
            ImportEquipmentItems();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Complete", "Item Data Import Completed!", "OK");
        }
    }

    // 1. 일반 아이템 임포트
    private void ImportGeneralItems()
    {
        List<Dictionary<string, string>> data = ParseCSV(m_generalCSVPath);
        if (data == null) return;

        string saveFolder = $"{m_savePathBase}/General";
        EnsureFolderExists(saveFolder);

        foreach (var row in data)
        {
            if (!int.TryParse(row["UniqueID"], out int id)) continue;

            string assetPath = $"{saveFolder}/Item_{id}.asset";
            GeneralItemDataSO asset = GetOrCreateAsset<GeneralItemDataSO>(assetPath);

            // 공통 데이터 파싱
            ParseCommonData(asset, row);

            // 일반 아이템 전용 데이터 파싱
            asset.IsConsumable = bool.Parse(row["IsConsumable"]);
            int.TryParse(row["MaxUsage"], out asset.MaxUsageCount);

            EditorUtility.SetDirty(asset);

            SetAddressable(asset, assetPath, $"Item_{id}", ADDRESSABLE_GROUP_NAME);
        }
        Debug.Log($"[General] {data.Count} items imported.");
    }

    // 2. 장비 아이템 임포트
    private void ImportEquipmentItems()
    {
        List<Dictionary<string, string>> data = ParseCSV(m_equipmentCSVPath);
        if (data == null) return;

        string saveFolder = $"{m_savePathBase}/Equipment";
        EnsureFolderExists(saveFolder);

        foreach (var row in data)
        {
            if (!int.TryParse(row["UniqueID"], out int id)) continue;

            string assetPath = $"{saveFolder}/Item_{id}.asset";
            EquipmentDataSO asset = GetOrCreateAsset<EquipmentDataSO>(assetPath);

            // 공통 데이터 파싱
            ParseCommonData(asset, row);

            // 장비 전용 데이터 파싱
            if (System.Enum.TryParse(row["EquipSlot"], out EEquipType slot))
                asset.EquipType = slot;

            float.TryParse(row["MaxDurability"], out asset.MaxDurability);

            // 스탯 파싱
            int.TryParse(row["Stat_MinAtk"], out asset.MinAtk);
            int.TryParse(row["Stat_MaxAtk"], out asset.MaxAtk);
            float.TryParse(row["Stat_AttackSpd"], out asset.AttackSpeed);
            int.TryParse(row["Stat_MinDefence"], out asset.MinDef);
            int.TryParse(row["Stat_MaxDefence"], out asset.MaxDef);

            EditorUtility.SetDirty(asset);
        }
        Debug.Log($"[Equipment] {data.Count} items imported.");
    }

    // 공통 데이터 및 분해 정보 파싱 (핵심 로직)
    private void ParseCommonData(ItemDataSO asset, Dictionary<string, string> row)
    {
        int.TryParse(row["UniqueID"], out asset.UniqueID);

        if (System.Enum.TryParse(row["Type"], out EItemType type))
            asset.ItemType = type;

        asset.ItemName = row["Name" + (row.ContainsKey("Name (string)") ? " (string)" : "")]; // CSV 헤더 이름 유동성 처리
        asset.Description = row["Desc" + (row.ContainsKey("Desc (string)") ? " (string)" : "")];
        asset.IconPath = row["Icon" + (row.ContainsKey("Icon (Path)") ? " (Path)" : "")];

        int.TryParse(row["Price"], out asset.Price);
        int.TryParse(row["MaxStack"], out asset.MaxStackSize);
        int.TryParse(row["Tier"], out asset.Tier);

        // Weight (CSV에 헤더는 있는데 빈 값일 수 있음 처리)
        if (row.ContainsKey("Weight") && !string.IsNullOrEmpty(row["Weight"]))
            float.TryParse(row["Weight"], out asset.Weight);

        // 아이콘 로드 (Assets/Resources/ 경로 가정)
        if (!string.IsNullOrEmpty(asset.IconPath))
        {
            string iconPath = asset.IconPath;
            if (asset.Icon == null)
            {
                // Resources가 아닌 일반 경로 로드 시도
                asset.Icon = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/00_StarVillage/Sprites/{iconPath}.png");

            }
        }

        // 분해 데이터 리스트 처리 (Flattened Columns -> List)
        bool.TryParse(row["Decomposable"], out asset.IsDecomposable);
        asset.DecomposeList.Clear();

        if (asset.IsDecomposable)
        {
            // 1~3번 슬롯 순회
            for (int i = 1; i <= 3; i++)
            {
                string idKey = $"DecomposeID_{i}";
                string minKey = $"Decompose_{i}_MinQuantity";
                string maxKey = $"Decompose_{i}_MaxQuantity";

                // 키가 존재하고 ID 값이 유효하다면 추가
                if (row.ContainsKey(idKey) && int.TryParse(row[idKey], out int resultId) && resultId != 0)
                {
                    DecomposeData data = new DecomposeData();
                    data.ResultItemID = resultId;
                    int.TryParse(row[minKey], out data.MinCount);
                    int.TryParse(row[maxKey], out data.MaxCount);
                    asset.DecomposeList.Add(data);
                }
            }
        }
    }
    // [핵심] 어드레서블 자동 등록 함수
    private void SetAddressable(Object asset, string assetPath, string address, string groupName)
    {
        // 1. 어드레서블 세팅 가져오기
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("Addressable Settings not found! Please create specific settings via Window > Asset Management > Addressables.");
            return;
        }

        // 2. 그룹 가져오기 (없으면 생성)
        AddressableAssetGroup group = settings.FindGroup(groupName);
        if (group == null)
        {
            group = settings.CreateGroup(groupName, false, false, true, null, typeof(UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema));
        }

        // 3. 에셋의 GUID 가져오기
        string guid = AssetDatabase.AssetPathToGUID(assetPath);

        // 4. 엔트리 생성 또는 이동
        AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);

        // 5. 주소(Address) 설정 (예: Item_1001) -> 이 문자열로 로드하게 됨
        entry.SetAddress(address);

        // (선택) 라벨 추가 가능
        // entry.SetLabel("Item", true, true);
    }

    // CSV 파싱 헬퍼 (따옴표 내 쉼표 처리 포함)
    private List<Dictionary<string, string>> ParseCSV(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"CSV file not found at: {path}");
            return null;
        }

        List<string> lines = new List<string>();

        try
        {
            // 1. FileShare.ReadWrite 옵션을 켜서 엑셀이 열려있어도 읽기 시도
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8)) // UTF-8 강제 지정 (엑셀 저장 시 CSV UTF-8 권장)
            {
                while (!sr.EndOfStream)
                {
                    lines.Add(sr.ReadLine());
                }
            }
        }
        catch (System.IO.IOException ex)
        {
            Debug.LogError($"파일을 읽을 수 없습니다. 엑셀이 편집 모드(셀 입력 중)인지 확인하세요.\nError: {ex.Message}");
            return null;
        }

        if (lines.Count < 2) return null;

        // 헤더 파싱
        // (주의: 엑셀 CSV 저장 시 첫 줄에 BOM 문자가 들어갈 수 있으므로 Trim 처리)
        string headerLine = lines[1];
        // 만약 첫 줄이 헤더라면 lines[0]으로 수정

        string[] headers = SplitCsvLine(headerLine);

        var list = new List<Dictionary<string, string>>();

        for (int i = 2; i < lines.Count; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            // 주석 처리된 줄(#으로 시작) 건너뛰기 기능 (필요 시)
            if (line.StartsWith("#") || line.StartsWith("//")) continue;

            string[] values = SplitCsvLine(line);
            var entry = new Dictionary<string, string>();

            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                // 헤더의 BOM 문자 제거 및 공백 제거
                string key = headers[j].Trim().Replace("\uFEFF", "");
                entry[key] = values[j];
            }
            list.Add(entry);
        }

        return list;
    }

    // 정규식을 이용한 CSV 라인 분할 (따옴표 안의 쉼표 무시)
    private string[] SplitCsvLine(string line)
    {
        // 쉼표로 나누되, 따옴표 안에 있는 쉼표는 무시하는 정규식
        string pattern = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
        string[] values = Regex.Split(line, pattern);

        for (int i = 0; i < values.Length; i++)
        {
            values[i] = values[i].Trim('\"'); // 따옴표 제거
        }
        return values;
    }

    // 에셋 생성 또는 로드 헬퍼
    private T GetOrCreateAsset<T>(string path) where T : ScriptableObject
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
        }
        return asset;
    }

    private void EnsureFolderExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}