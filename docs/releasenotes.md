# HEAD

On-page editing rebuilt from scratch with new control panel (bottom left). Auto-save improvements and stability. Editable children restyled and cause save to draft during operations. UI texts editing.

```
26b45a6fdf139117357c8164559b395b099fa077 Merge pull request #715 from ToniMontana/MongoLinqQueryFacade
8a74177757fe35c7534f5a8e3c3f9ae47828b97e N2.Persistence.MongoDB - MongoLinqQueryFacade: One powerful feature of N2CMS is to be able to to Linq queries against the documents but this feature seems missing in the MongoDB implementation. This commit implements LinqQueryFacade for MongoDB and makes it possible to do Linq queries when using MongoDB.
d6dc926cd83eab1086cc614cfe1263425ff655cd Merge branch 'master' of https://github.com/n2cms/n2cms.git
d49d3650f404251a365c809b89ccf52d7d23e44d Fixes problem with editable children and new items (fixes #712)
a3b25ffc50efbcbcb661ba081c3c46dc27f7181f Merge pull request #713 from gibgibber/fix/solution-is-broken-in-folder-with-spaces
f1dd3aa5078f45c8d9a8567b18cc817cd48a9ad4 update post build actions to avoid xcopy crash in case solution is located in the folder that have spaces in path
6463df178488bd10544f6e703d30ff0ba22eac0d Avoid overwriting meta info about enum values (fixes #710)
77311a583c6f3c437039c3e63087a4fc095ef5b3 Merge pull request #708 from Tazer/master
5183cea9a7d62c05e0c8a571b99b6b288ed94fc4 Wrong/Old namespace for IconHandler.ashx
07ae6f9af2831bf640fb27d62a871e3ba4e74516 Editable css tweaks
695605bf5b06481712e5143a8d0c3719e011e6e9 Part styling and hover effects
9404c81097c0b454616e06613d9a7108db49a7d0 Scroll part into view (fixes #707)
c494892451c38d4fce547a156e4b51667807de9e Removes editable wrappers from keywords
0a3fcdedd945835331bc2f325cf5edfec72c9acb Removes hello world text from dinamico
3784e1def30adf957ebb1321d82e4190b30e88be Resolve control panel dependencies on server to allow configuring alternative resource paths
8e4202bfc86d4d71c75d9618f1a6279ccbe0920e Improved handling for links in editable media attribute
619bdc381fc52aa10e1405e30e8e051930f83517 Handle exception when resolving external urls in editable media attribute and fallback to image
bfa1186c0e538135a07c5f7b4af74eee3634a0ff Editable html element detail
9b8449bce36481e83e45b3e2382146071a660417 Fixing cache section varyByCustom and varyByHeaders
40e2357d2d76cb8eec6463a5673eb476ac00864f preview CSS tweak
27164168269d817d7f8de889c443a164c61057d2 Handle running control panel from a virtual directory (fixes #703)
e840f01e899df132cc0e53289bb20469eda28525 Improvements of new control panel styles
221f92f86aa09ac646faf63f90c1381f32434b37 Restore CSS for legacy control panel
9957068bf2faf4ef92a4083ff148cdf046042c39 Assembly version 2.9.6.2
bd65888a94b2c564bb1087075bb63b61886e3f0d Fixes problem where editing an editable could save changes to unpublished version
378c35fa8232d2bf67f7cf3ced5e74aaba827d60 Using new parts icon on show parts
46a53375ed5f565b25034ce2a23126f3c40c8fa7 Avoid warning about autosaved draft when translating
b871ad579f5044e3e22de6349ac344d7f484828d Relink items to save to master version (this solves issue with editable children on page)
cb4682ae1ac677fad65306c52eb4ebfc39fe45a2 Removes draft coloring for system nodes
0b649cb2e878deb06b2f4202e722f4b7bba089c5 Removing f
2b92aff737d42a7c83b5f11cb19a12e2ab57d31f Restoring misteriously disappeared section
b51ee8119402c471fa1d224ad8cc7695ad6fb234 Render part within it's droppable box when using @Html.DroppableZone("ZoneName") (fixes #702)
bcd675a140d06af426cd90b6905b5096f20b9c84 Restoring changeme password
9209bd5255d9a1ac7e7961a9ea6b49896b47fbf6 Fixes problem with autosave notification
d0ac1fdfc09e3e6348c721d705f178ab605f1581 Assembly version 2.9.6.1
200ffea002057fbd10c302da4c2bd6cd4c60551e Maintain auto-save id when posting items causing validation errors
045edab67f13fe9eb15316da20e13d46939acd75 Assembly version 2.9.6
a76b9e7e0ccaba137d1d5364d9f219a2201d0ca7 Adding EditingInstructions to definition which is a text always displayed when editing a type
8e982172cb091744a3abebe07f37a412d3ccf22f Alert styles
23ffbcbb3513924f0ef8058cd0f550f6bc44307f Update title from autosave
4649270047359cbf0b512f482befda3d2c2bfb53 Notification icon support
9592452013fca6eea72386db8e21ba80446314cd Changed notification appearance
4dd6eda614f0627cf5d3b13a78d621bee154d74d Simplify discarding autosaved new items
081b7af22438b174911d4614061f961ec647b981 Autosave left behind notification with option to discard it
42b15edfc03a88e8e5299b79fec95feae96a75e9 Autosave refreshes site tree
3b0c9d32c50bf4fbe2e9dacfd0f4b204b0217aa0 Allow draft preview under some specifc conditions
0bbdcc970aeb95b30d442fd0aff22c688693509d Removes console logs
23658abfcd33dc200410adfbbb03415961dac87d Improved loader screen
8545bdbb7e5594305824ba09c0ad9cc984ffa97a Merge pull request #700 from jsassner/master
9295a972f69a1f6ad8adfd07e064ebaa35a8478e Use page urls from config file instead of having them hardcoded. Added url to where to manage security to the config path element.
11dc4346d9f48b56246cb1265b00ad4cbf6b6d3f Merge pull request #699 from jsassner/master
13f2cc05925aa2309cb94f372e5e2d5c6bfb2af2 Made GetPermissions virtual. Made GetPermiossions protected.
196a24a9a626f2bd65f0ef31e83478e26ca5c014 HelpText on definitions which enables help icon close to the page heading (fixes #697)
f22f3872c72a160232a40723b8ab128337fe5d13 Direct editing css tweaks
7976962668cc2de7434e969f6d4de87c4a40a402 Allow editing on all registered editables by default
60a07d6df1eeefde019c19923e643c768084b4a8 Editable children styling
51bb1f3b57c1d8e8a1c000a5939deb8fc6c53f07 Update from existing editors before executing move/add/remove on editable children
cb4579f46b1a734881d4b0499807d26d81e2d5ab Assembly version 2.9.6-beta2
b7e92805bddbb7888e6d1a91e96629e3ab59fedb Immediately add new items to a draft when adding parts to editable children
ae185d5f33fce39eb395cb5cc8285e7db0237b8e Slider shows parts from draft if this is previewed
9d3c8dc965edacd321b209a2f8ffcd5046af2234 Changed behavior when removing and sorting items in editable children. Now a draft is saved and the change is applied to the draft immediately upon click
a8f08b6f0051a88e44fffa69062e1434a3a531a4 Autocomplete styling
b2a0477621a350ccc420f8562ef221923b847a88 Use ie.Definition.Title instead of ie.Title
8d81fb7cd6853d516cfe724d5b17e477e166b43d Dark control panel shadow
92758197ca8f3fb1017e188ba1c77db562614644 Resolves some js errors when using root page
d580c9afeee88e72dec861536115ef4680663e4c Font sizing
a127000ffc52477296dcee9832fbd1c10361d639 Hide editable wrapper unless in organize mode
8fa7c7d88ed49e1d4804fbc381c3b2d773be3157 Link to existing draft from control panel
75799dc805803d9fb8b7be626280afd5ea0fc6e3 Improved icons on version list
fcac2d47c612a4cbaebff620b104acc1d8569119 Assembly version 2.9.6
426dd8a892bb17a5fb504831f22d1ed4438816f8 Merge remote-tracking branch 'refs/remotes/origin/ngpreview'
810e466740736bae43bf726c71246853c0e5b4fd Assembly version 2.9.5.5
e0a3b27af932d669a11ac00d6556f4da88806fa4 Disable publish while autosaving is in progress and simplify discarding changes done by autosave (fixes #695)
35f614a6f2bf51f91695c71961f73110ea507a82 More options menu in preview frame
75bfd6ab184041bd88b065617c77aca1cfba0efb Property editing
e1573b8d8d4d7cefbae5dbfa54265d7d7d29cdbb Refactored preview services
868c94ef84aec8f1a25e339308bdecf7d786279c Polished preview adding/move experience
b8b588671607b43309dde96f1984401062576a3b Move parts using new control panel
129e106ab8ffeb3b223ca3589a314c67f96821fa Fix insert before part
a874288f4cf3cdf4d0af0d97014050eee1edf9b3 Striped drop area
5b3c443b270f90c75a1b0bfd599e3fc63023657a Icons on parts, new control panel on webforms
acedc50e3744f878f9dbfb2a49ee25a091a7ef89 Enables upgrade without session state enabled
16d4d9cd4ddf4d9ef255833d4c77c70cc342db36 Removes deprecated reference
77d876c41d68e9ff00af414fd0cd4e68149c57cf Spinning loading screen
89119a034fb7546ffb676e9cc8aa448956e18fb2 ungroup icon into group icon
5697770163964ad529fc2597636138627790d57f Changed move into organize
e4b42e5a93e6b859c18139c79252abce0b02430a Organize/move existing parts
c3702ba240f1ef5e2a239295d713bbc506741bd2 Reorganized preview frame controllers
0e5d0a890ed8329266f3f23dd0fe82308338f663 Return to organize mode when saving part
2aca62e2bfabdfd864436d2b6595fb9772f64d83 Organize UI
df381849249f4696eed8bbae70a96d1af293e97d Progress editing parts
f7974785b465b0fa5729c3cf6ee32593bf921451 Merge branch 'master' into ngpreview
6eb13ff66fe0bc90250ddc5a6b300d9456c74f82 Merge remote-tracking branch 'refs/remotes/origin/master' into ngpreview
b969cdd310fe693b21182508a7323fc9ce70058d Updating .gitignore
866cc75bd807382b0e9db0305c2c1e9d6e5fca29 Improved guard against empty translation keys
1f4bdffe016dcc5f9f164acb4d46a23d182bd789 Revert "Guard against empty translation keys"
3cca08ba4e79a4bdcb242330089210f3ea6d1e05 Guard against empty translation keys
6e4b09ac01af63cd73cb11b2ddf7c46cebc599db Assembly version 2.9.5.4
697047007e4bdb729fd9443a112522d460e682a1 Fixes problem pubilshing from the top button
03101579e55d5cce893d41940d52a49816f4584f Fixes problem where meta information would be lost on previewing draft causing custom translations to disappear
799ebbfdbb21c745d477f14cf83b5d96d39e929c Improves upon adding part UI
74518fd39a91ae95b051870cc74bf81fb496c7a0 Improvements to the new control panel
6723710a87d5577c857712a380ddc48872976ae0 Prettier error reports
9ef153239d05cae043722e0a2fa04abf53720fd2 Improved handling for empty translations
4a0db299c2c5ad32936caf45783f1ea378a13af2 Assembly version 2.9.5.3
156bf7edce506b3debdd36ca8bae84e38ca06e78 Progress on ngcontrolpanel
6a5e54839631f144a714ce88b94d902237352dc6 Additional merge changes
650663291fa0aad2e524759a16234d5b7c33fde2 Merge remote-tracking branch 'refs/remotes/origin/master' into ngpreview
0d5afb27e69452662c487201c3734787d2e81430 Assembly version 2.9.5.2
4cbd599ad9568827fa1b4640a1612a46fe5ddda6 Editor for custom translations
9de92732d0e768191da01744d38228f2f752f3e7 Assembly version 2.9.5.1, tweak of build of Razor package
d742c3dbcd858c4a83f31cd8fdb4990111a1ed9d Login page
c45a6d6f8679742b4515eacb0b4a4a6bf7f4cf9b Simplify specifying rows when registering multiline text
19576d5fd8260d0acf05a1545a73a475ba1041f4 Removed usages of ?. expressions in code.
46049f871d6df81b7f149a062bef0fa99036cde3 Assembly version 2.9.5
5bfb895c2af00cf32ba42617fff8895ba36b6c30 Fixes problem where translations would be represented multiple times on the site
7882545591c6a1e29dc623b06d6d9339aee2b7d9 [WithTranslations] to edit content translations
97dc6c0c6f56b16519d41a092cca46138e58dffa The ContentTranslator service which builds up a collection of missing translations
a233ae8dd8188b396b5189b9c5c414f200e75ad2 First appearance of ITranslator
026aeded7f14d6d4c96db45aaeea16e8cecaa03c Initial stab at new control panel
396a17e68f67f4e043fb2d548d1bbc03263d6676 Fixes double jquery registration problem
dc21ea47a702e444d3c6643e9847e0add83f03de Fixes problem whre saved items are detached
f1d04cf3a289f94a6575b539be74ee01469fe478 Editable file content selector
4fa46cb96ac3e72f0ecce258565f31df4d5b6912 Update mvc version in web.config
110c7c18aab2303300153c63ba55f3a01a5d9b12 Support for filtering editable children by template key
d78448ae5575aa8b55e65fcf9bd0ffd25ba93588 Removes problem related to autosave causing empty item to be created (fixes #696)
1fad9cf88a2d6cea48ea696dbe36f003681de02a Removes unused member
0757a0faab17122e55993b1c5a5f9ba173bad8ab Disables autosave for name
0e658b1d9c257246f08bc88033b88953237f726a Simplyfy outputting data from template-first registration
1084acd0d8d4ff9d8b96ed10e9387e6e192843f1 Support overriding view template registrations from site root views
2f2958f2c218130516ab0c15f8028db9cd395171 Deprecates registration Title, IconUrl and IconClass (Use Definition.Title, Definition.IconUrl and Definition.IconClass Support
2afedef9a03ebb0ee3207f6fbdcd85f7adaed117 Using new and shiny render method
377f337f6d0a0beba95b4140169e9aaf238c2c6e Simplify configuring multiline
725f032b0e335666611ac46b14c82d785c674888 Return value on render
62b36df2f6c967244b4a89ac40884dcfab33daa5 Include mvc version in web.config to gain intellisense
916dc9a7beb4f4b367e23cf3b2a8a8a0391564f2 Use view context state bag instead of httpcontext when registering style & script dependencies
d59250ae48214b5cd6484834f0f6ad80b4442d72 Assembly version 2.9.4
16d7e65f49070fd9105acbf42559906e8a11da14 Adding some assemblies for exclution from type scanning
36cc75d98c8bd23a5e8ea795c68683d238969a25 Prefer root views over default theme views when resolving views
c5a27b356452aa82a37b571919ff6f4e55267966 Part rendering html extension
9b3a15050d81abf43f2254f9bbac05e8685fb3cc Allows clearing date on editable date by clearin date input (fixes #694)
8c5c987c83aea49de06c43e1a277a412faa17212 CKEditor allowedContent true by default
76651c5bbf9e83a123067851b429c5b98d5bc1b1 Adds fluent registration support for WithTokens to text editablse
0422297c67586577f0c39a3481ea561c7a9773a7 Fixes required on editableitem
1d5ab96c54c2f1c2681b34276e42797269e82ac6 Fixes dinamico redirect icon and actual redirect
e9d9cc0412fe6ad2763e51eccb1b8b4e4f523ef0 Assembly version 2.9.3
0ebf3b7830779f804921336ab1223147417bc898 Missing dinamico project file changes
d5ccbe5130258a3a513a51e5cd36066da6f51fee Removes redirect template and fixes brushes up content item on dinamico templates
1215ec8dd70a98168ed9f5ab9215aaabff06739b Fixes required [EditableUrl]
b21fbc47fc729562fd665537ab3129bdeac8b2c4 Fixes TransientObjectException  when using a combination of [WithEditableChild] and [EditableChildren] (fixes #689)
d3a1468b4241a122fbfc5b81c308358272ce4f23 Media browser fallback texts when resources are missing (e.g. zip deploy)
6b45362a4327aee687775a933e4a9202900d9ee9 Assembly version 2.9.2
23dfae585f7329b2498a2f7a78a6e84cf0f88118 Fixes required on EditableMedia attribute (fixes #688)
40e13792ad742898ba64930e19bf4cb6327deea7 Rearranged upload controls, added upload button
6883a08bf3e64ea578b2bb9f8abae466a63873c3 Fixes javascript error when selecting file in new media browser
229805fb537df5cd33b4995ab5e3ecc696460a7a Merge branch 'master-n2' of git://github.com/SntsDev/n2cms into SntsDev-master-n2
fc5ae3aa645dcb997f236e42db0fdc522379f17e Improving preparesources and nugetbuild from clean clone
7c0c58309a4a4a140ce5782c4162f946f19e8b38 Trace false
e57e8a7901bd41426d363ddec063b0c70846a449 Check MaxRequestLength for all files (not individual files only)
77db41a03412c4861affad44eaa0922e36305059 Trying to correct Resources bug & improvement
442658dddb3082707494782ec04457581e1b4b71 Missing gif included in csproj
a4a01d5118aedb1687ad6ad240df9a2e93e98244 JS executing before dom-ready
16a7c65485cb3f028df3a2d21f4877f974dbdace Removing removed file references from csproj
2ab918a4b5429010657891c75b5645550a0ebb6f Fixes problem related to multiple controls sharing id "input"
80af73a18c7b88cac778489f054e6eed1cf5bd6c Assembly version 2.9.1
1fde3df6f173de86fdbfd46201c3e50ded5bfdbf Fixes javascript error when clearing media selection
b6940bd032b1002af5ec63302df241740467a0a5 Merge branch 'master-n2' of https://github.com/SntsDev/n2cms into SntsDev-master-n2
5d8f8264db96f98ff4151b676318ad0824a8d87b Restored file browser
be6cf8ece5a5b496df4e577d0436deb8fa019dc0 CKEditor 45.4
66e73e6195cb369d8786e64e15f5f871bc33a5be Media browser improvements
a8b200cf86dbc723bb4048178985f4d71fe7721c Commenting line media validation related to upload causing compile error
a9409f134b8234642a30002e8e3c39e155f32e28 Merge branch 'master' of github.com:n2cms/n2cms
923d4b4e6f44a80ca9b924ca88105b62d3d3208e Merge pull request #680 from SntsDev/master-n2
ddf58b847e526c62d6bdb69ac0c3da6334dfae16 Update EditableMediaAttribute.cs
f7241acd8d407ea7659c81c90daba22d7a76a1c9 Including mediabrowser.css in project file
b0b92d0a731b8ef8163505e3f65939ff6ecbea8f GetLocalResourceObject -> GetLocalResourceString to support zip deployment
b006477963aa50122ae458e7d096b65fc87a45b5 Include media browser resource file in project
021648b401183ea00e1ab643bf86503345c9a4f0 Assembly version 2.9.0.1
20cb7f02dca4c30bee650998fd866b697508cd05 MediaBrowser.aspx.resx included into project
0e6f06cbc23173fddeae7569a8ff9169f2529196 MediaSelection.js included in csproj
81eb9a280d7db55ae57053562b05e485fb6149f7 Including mediaselector.js in project file
```


# 2.9

File selection using a new media browser. Popularity page sorting. Dinamico templates improvements.

```
a8aa89ee1e7e788988e4a4b300ea42c77e0d9a50 Assembly version 2.9
ca9a5a19645e17fc5a69a496b8d65c845c253ffe Fixes problem related to auto-save when using [EditableChildren]
55c7b10228c816531918f3d40575973f233f975a Angular into framed template
f5348338b4a838d5dccf3f87004254a0c4b5acc6 Moved media browser select button to the bottom
084c7bdaceaeb88b698a7c022a33efe7cb31333d Merge branch 'newmediabrowser'
5eef6fd28a70402727f5c31b6a9c1402f3419156 Polished url and other selector inputs
9ad283a2fcc5fe10aed69c5e07e9966eaa64ef0f Styling media selector input field
d1912a649a5ff222f93ac3e2abc42408852388ac Converging multiple editables dealing with file selection towards the new media browser
aa3b094148c964342e025867a928ebafd5b4e2e9 Tag on push
a36d4f5333bf6551ccf3e3e825922db5b515bbe4 Assembly version 2.8.4
168d4ec4f338a3756bdc2b657afb11e469864e98 Keep dinamico 2 columns down to small devices
6b496eef75e57230220f28e3dd538debdc51b098 Dinamico tweaks
91ff24360f8e43e2dc7de2845191feb66602e97b Fixing saving nested parts (fixes 677)
24ccb13fad95e241ca431630d560355358e309b7 Dinamico column fixes
a49999b12be021bc20e84c62298da55a8c12b5be Encode exceptions logged in management
02eeffc7fec07396caa1099c8282899aaa0efdbc Fixes content messages when using windsor container
356cefc59d142c03e0a26f807b375db211d269a1 Merge branch 'master' of https://github.com/SntsDev/n2cms into SntsDev-master
e426a3f77b012d1f571444546eb0b898b60e0a3b MediaBrowser
ba7d7b37b7fecbd1599a24ac89c2382970a2bc80 Updated statistics package title
21e4c8d54f4514b3b1e61d4c599a5afd4535d162 Assembly version 2.8.3.1
22d91d7976f500e674ec222803a7994a3af01a27 Handle null user agent in statistics collector
81d74e474eb6f4847673602309dff494989493db Changed demo project references to project references
5059df1f0f9237c1e6f4c979fa9bde38b72af759 Fixed parts links from management tree after recent changes
079af62bacbd9513de7abc2c54035bcf3b457bd0 Pre-compute preview url
b011fc1696b575deb4ed102cbb1194d51a308583 Assembly version 2.8.3
348c155e859ac921b5205ed4ae9093c1bdb3a9ce Dropdown autosave support
3d3eaa162b189519cfd8e146423431d0dd9f3a4f Improved parts handling via management tree; selection, zone placeholders interaction
80eb79cbfc79326698237e3ce25fe9394cb37216 Support for removing statistics
f5dbd2b6951135d1eb9df599cfcc4e9a8c1fa64f Restore selection via management navigation
4a0099c6b0f2c393f9675f398bb4371d80c025b1 Tweaking management context handling
910f5a9acefe705a5c937bd2f19395fbfe5512ea Consolidated preiew frame handling
669b0c0a1a43426be63fb4fcd1346f6bc46fd46a Remove console log
1f9b27cea086b9fcb6daf1c55305be11753d602e Exclude spiders from statistics
825f21c75750a4cfea6358f2db5b807a0b0fe92c Context menu doesn't automatically select node
316f577b094d59258d93d6d8c8fe1e8797ae5823 Same shared assembly version on multiple assemblies
4a931ecabfefa39f4af7ab8e3557e0891c5b5be3 Changed defaut namespace of N2.Management
d0f240bbbb33a1dca31a3a684630b6ac8521ff20 Reduced warnings
e6db36c7e275355539fda8b3bc3a75b441dd3c20 Compile against 4.5 rather than 4.5.2
c5534e95b152eb88f04055b80a9d7a762d148ade Assembly version 2.8.2
953dec9a7b8440eac1657842a6ebf8679b901fd0 Tweak page stats panel
96146fdb4fa700885474b4f5855d5710770a8102 Sort children by popularity sorting
12955a0e3532e12d9b487816a3a1a71153ec7fec Diagnostic info about scheduled actions
e952b83e10cb8443236d3f51d17428409d88147b Support for custom children sorting
cc312a80d19171cb3f6075723db9efc66ddd4bc0 Statistics tables in dinamico sqlite development database
534b3acbaf2060224bf29c83b5c3d46c214d40b6 Assembly version 2.8.1
bf270c1aed15480c1661fe8b497517686fce9ee4 Reworking deployment scripts
c6f0c1c12015ced8fd79e8f2659dbdb78c86889a Moved management project to N2 folder
ad659e337beea94fa42fbe958240304c31083a45 Changed preview image when importing dinamico sample content
074120d3dc2b32deaf310fe6d0200a7487d4a83a Delay messages monitoring until database is online
3eccf1131e5d9897a2d2b779783de4b040fef5f8 Complete moving messages to management
885bac59078ad98e7c745321ee261304ce85403e Statistics tables capitalization
db9468fa037e595e99c36069e7188b564532fd8b Moved messages back to management
70af41d3e47ecf0071712055a3421cd6fefe49d5 Merge pull request #2 from n2cms/master
```


# 2.8

Auto-save during page editing. Editor collaboration using page notes. Built-in page statistics. Improved keyboard support. Management messages. Output cache invalidatio modes (site, sitesection, page, ignorechanges, all). Dynamic targeting module.

```
5d82c1b440f49f2ef3cc176ed0e35a7be7a16428 Improvements to editable number: support nulls, template first registration, not required by default (fixes #673)
504323165929d7cb8d42d80c1c84ba7c069583f8 Removed missing file from project
ec38de17f44a1d4be22c5862a24e6932cb2b1f62 Fixed some unit tests
0df0aa4fe8e0eaeb30ace2b2f47c3f570cbb30ee Missing files collaboration project
3325c2dfdfd5aad2cced963e045d4fb91ceed895 Assembly version 2.8
a29d8c857172cd24a776e9fd1489f424ade7f610 Support for "shutdown editing" which warns user before publishing changes
a790f6cbf870323a5cab277f31c75d3a745058d5 Management flag refactored
5a33b61a5d0fb2d25da6b06d81ce50be1e6573e8 Note height adjustment
d669dfac6aaaa6854611cae0298f7e1c1d4eaf6b Collaboration page notes
7a6559c2ae6da1e68046a6328ce332688ca159ad Item annotation support
5a6df7c4cb442137ad6d7bbca596b15395a013db Removed comments
4a329d02b2df09e096f5e035b60eb13d50f0862e Friendlier action-style api for content handler actions
df31e5da11fc97f9a027c8c9ceb92e13e80bdcd7 Really removed collaboration css
579bc5c6475fb6ee4dcdd061eae3b5890a5f9ed7 Removed collaboration css
83fe0b716d5f67e49d21b3fe3bcd65218eae6f54 Collaboration interface moved to separate nuget
b98c0b6e6537ac3ba10cfb2d4c10330b2a3ef4c3 Updated resources xsd intellisese
ba29251521e9dc451b4f52d14eea222a03c767b0 Auto save interval, multiple change behaviour and only autosave on page editing
7a47e59f98566d70b834213b9202dbfae5d56c6b Makes sure name is populated and handles scenario of editing draft master item
41fe8a98ccd71c933091ec6e8d7f5a427c76bb8c Improved auto-save handles new items
73e2cdeb22f52e1b6c23cf43c61f5ec073dc310e Content link tweak in dinamico
103d5f10ae7362032063b7d52b5178d58181d200 Barely working auto-save
1ed1c004648e20bb3daa7649b7b2f67adbd5fc51 Statistics display tweak
18be27a94188f9e2dbbdcb1dec3e35a67f062696 Error message tweak
4d7f170a97fef4947eeffed7cf76c979434b542b Reduced logging level for some messages
a22a068f5ff166d25ed8e3789c41f18286ed0454 Report logged errors in messaging area
e9d366042eaf15cdcd145621d5c5c790b1238a82 Visual improvements to the upgrade page
77ced31c34fa9294188adaa343f1644645611dc6 Display message about missing statistics tables in management interface
05b27fede53b8957048ca664e2e2633748004bd6 Merge branch 'popularity'
0ea1e8819f3d65da696cbad6df1234c34bfa2021 Fixes closing future publish not working (fixes #672)
24ca9afbb9baddab65135b4e3a8bf7b9c657d56b Merge pull request #670 from nimore/LanguageCodeSearchFix
030c4882b3235f7b118f6b96c855e7b5acfdf6f2 Avoid loading statistics on closed info panel
05869923d2d8e9d8ff062591bfcc1dd9023c87fb Configurable and more frequent transfer interval
258d1942d221bcdabbc523e35d7f5e26ea68bdd7 Display no data when no data
81591cd57cb44c70e4e74fb2277d21aa38b41ce6 Assembly version 2.7.6
302cf548eba96c859a0b926e0ede05ba29068fe1 Statistics nuget package
223bccb071f0fde25d1e3ce3e89ae0a14074629b Missing files
18f6a24ec1f02c30ed27d585f1f4390d1c941ace Fixed statistics retrieval, transaction while transferring statistics
56e05647d8349431b22e7f9e2bf8d7196221ee2b Actually adhere to config
4308d0cc0c09adfa973d874e085a3fad671733a2 Statistics hover info
00e6d9ce36aaafd5e001dbcefdd798f2eee6b263 Css tweaks
d10e5c0f66e6387aefb8d256f9319cd577b43ae4 Improved statistics presentation and granularity config
896eac1b6b46ff88d8877ec5ea3d769793730bd7 Display view stats in info menu
292f8cbc97fd210c39f3bdc7dc2750611bf58d09 Statistics API
8f679367871ec9a274c9aa70d14e994b4e0518d1 Statistics synchronization
b0c3558b847bb549b2239b8c87fc037c21d3cada Statistics collerctor
008f0726160bfa623efb6795456e35fc6e8416d2 Fix language searching when using a full (hyphenated) language code.
d442428a8b5594dadde1466a313012f0b22fbb43 Renamed Google+1 token to GooglePlus
ad8676878331761f2cfbacd7d058848d7e478ad5 Improved keyboard navigation in management related to context menu
0fcf2aa9f88c6adcd0561b4f41c251d89f48d0f2 Keyboard navigation in management tree
0bee7f1eca75b364e93fc6d1854353de90e2f695 Tweak navigation css increasing click area
03a4119d9414edc9521b4362a82ac085d3f9106d Removed "scope from here"
d6175a528f75f476fb2285f45d846b505720ffaf Add missing project file changes from previous commit
334520cdcaa791c180ac42af510e2b06cca59c37 Show wizards in add menu
2068380908645cd9c3c8dec7318284f3a76f4466 Some targeting module improvements
8cc5c3c8d435dc67d75d21a574def3df8af641ad Interface builder restructuring
22e305e1eb8584be3e512ea872feb2aedce46fbf Documentation
c661c2a0b48938cf536bc046b0cbd931833fb825 Merge pull request #659 from kodeo/master
cb2862dbb8de1637402efcf885eb9969b743b387 Tidy up source
91965a99a2146e580ab4977774174e0a04ff76bd VS2015 user-specific files git-ignored
f54a76ddac166ff77a2f4d32786e2c35d7b447f7 Documentation
f79a2056de0b0ae22a7afa00ba452181f302e979 adding missing fa class
42519c924d3d3a2b4816524890a99b018dbcfdf7 Configure zone when using Children registration extension (fixes 654)
33cdc2e186914ddbe8d23721b835405d221028a8 Merge branch 'master' of github.com:n2cms/n2cms
d72cd893c8e179f5d2b57ab857c53181716c9c9e Assembly version 2.7.5
1b7a8dd18a6a6024bb46cce8a5a348ea20fb0f06 Updated dinamico readme
861be93f1b09e8d06f4b9037e422f5bbd0c6af83 Project changes
f20192b1132ae70a14e0c3aa1cc0829f34d0f424 CurrentDate and CurrentTime token templates
bbc6742222ed153a61d3712f1f20de58bf39ebf4 Avoid null reference exception when setting up output cache
e120e2bd1208ac8fcc438e7b83642164f1ce61dc Fix control panel positioning
346893589ea1044805748f2a3236222a82a56741 Font awesome 4.3
483028f00b1b77c4f90daeecca69f41af5e036d6 Documentation
c55fde1c2f53e1418417a6baabdb4b7db8f68bf9 Merge pull request #655 from distransient/patch-1
710643b2095d28e3e15e8e716236852ebe020e3e Correct bad Confluence api reference link
62b9b44d21fab02a68e8d1616f33434c785c8924 Null definition handling
355672fc5b3eda698cfcb78f270a01c536556a68 Changed ResourceHelper to use ToString or explicit Render to better support MVC5 partials
88a871ebb9a1d4deb451c5755f6240fa14495373 Assembly version 2.7.4
4dc9972f8f36199c80e7227d9cb512c096e305b6 Sorted files when file system delivers unordered files which fixes problems with image sizes
96fb14b040addb6027a937ae8ffbafdba2321195 Tweaks to dinamico: - Scripts zone is defined - Script zone renders parts from start page - Pre & PostContent zones defined
00c92ae3509ca37bfe8b8a141e0b6be9edc60b17 Merge pull request #650 from gibgibber/master
c44504d4cd19c3184c85665cf686b8dbd45490a5 Reduce locking in view template provider
4f48e6ef84da3b5f5444d4fa250636646e12e7ec Resource include tweak
e56687d7f0080f9e8400917de0cc50e70e586efc Fixed liveloop issue with file list
381258d5824f6ea9d33bd8b2b8d502513b6b2e2d use HttpRuntime instead of Request, because it is not available when Application starts
33f63247cfad14f774d19aae0af0e3cb526ef909 Removed mispaced png
8e62b8f893f413e650bb01e1a96506b71a25439d Reverted previous commits
0ba35eea2489f84398da77d798c4308df9b5efef Ignore files starting with _ when analyzing template-first templates
d2876e84ac8e0b266d24c0d1473173f13104aef6 Refactored previous commit
f89b0d86743787bace5a6eab2a554aa7b03e471f @Content.Has always returns true during background template-first discovery
cf2525a4d6d0b567c518af8300defc5463c96a7b Refactoring related to allowing creating templates from more places
d138d3eb5a85399c7ab4fa2cb7185b9e2bf713c0 Tweak part css and target container icon
bb5710b0a0711d780f9403211e69b594e4b9b40d Support adding templates through top dropdown menu
18e8f5bb5ce2e2207641b9092ba1d8837d2d3185 Removed registration for ContentPage
a7fe0ca7602e0fc63eee66d815c83f5fd3961fca Render single part
bc2bb4a77199de249ae5e90b0855a5f25ee9a44e Support @Html.DroppableZone() without escaping html
163cb4c5d2eca50a85a13d63b7b14f2caa54bcc7 Check directory exists in ListFiles utility method
cee8e99ffee0a17fb84a05a806779874cef62b2a Support for using templates when using [EditableChildren]
5826114d5c18140b61caa3754b8df73699747940 API suport for resolving template defintions
5387e752ffa730ff1cdddfb9c60196316b2f93a9 Heading level on editable title
abda2933305eed24a9b64e246d93be557ebbe1be Support for resolving template definition through definition manager
4bd06861293d2e8e15559a0d411e33c7cfd11bee Made some arguments optional on management paths
6b8698468b61cff0ee1825644b05b63d011bf7e8 Cleanup dinamico default theme and added MyTheme example
58a931f68be2fc69eeb193cf604ef8ea32f4c6cb Remove deprecated dinamico themes
00e86ade8fc48d1723a136f57b63a71a2db95ad9 Assembly version 2.7.3
b4221019b16f85e1359e844e496f271c9a00663b Allow dinamico start page without language intersection
9c0a08159a037bba4f2156de2e72c2fd5c3bdef2 Exclude user as root page
212c794175e2b42684e24f28c197f9fe1a43596f Improved index info
b288232be8550367f9dc6c59f426da9da4e12e11 Scroll to previous position when adding parts (fixes #639)
ed25591aa3585c8eeaf21022383fb382a7e53e88 Restore scroll position when dragging parts
cadb3b4f66dd049241ed07759cd68b686cbd4ac6 Assembly version 2.7.2
100ce7497d9b72d865e6bad24dedcef26c89295d Support for specific connection string in N2 config section
ebcd752c6616d11505cc4a37f60781fbbbdaf2c1 Move top level menu to fixed upper nav
6c26d9456f3ec2eeb3c68e9864f4661dbd4157e3 Remove expiry date when publishing page via editing (fixes #640)
66aded6836eb6c9d3804a550ac276d5a664fe24c Restored sqlite width interop
75a7928ad1e7815c56c2789b132791f5330b751a Merge branch 'master' of github.com:n2cms/n2cms
dc23cf6f48842f44713ec69ecb840ece5eb19098 Fix issue with GetChildren on globalization page (fixes #636)
57714fdafd0a8274213971152d475cc017bf1c80 Merge pull request #635 from DejanMilicic/master
739d92b557b399d37d64f035b642745121ef321c Documentation : server requirements
2d3f87d7082801e8bdbdcadf3a7b31b98237c3ed Merge pull request #634 from DejanMilicic/master
500caa01610f360c869f03be92b9f14ded1b3eef Getting started with documentation
f162d43cec51e9cf578904b68fd6d0a15063b3e3 Getting started with documentation
f4b0af6bd32d1d65140b17c1e11dbf26c91b651f Getting started with documentation
81e40305eff096a8f985e9910c6af294a9dd6eb3 Getting started with documentation
7557f599717e1c85ca27d61c1e40407ac06d126c Getting started with documentation
ef3947e50acb14b4c5bc250b6911d8c4bea18189 Getting started with documentation
06ef58d4da784a418cdaf7890f3ec7abf2aedcc0 Getting started with documentation
f926a07afc6bf20317d6b38bd2e6b76386e1f3a5 Getting started with documentation
7cf2ada361118babf0e5817568ef8ff97111d1bb Getting started with documentation
7648a25faeed2f2bb6c617ebef9585112513777e Getting started with documentation
eaab29c9531d6d488a14c2400ca6069261b387b6 Getting started with documentation
68e6629af2ef18839d482a175829d9ac21417ec0 Getting started with documentation
7b53ceb71d4b635d3a2c18f77652392ceb704b44 Getting started with documentation
92b73a99b6a38d6517b83624a5a045ea81f38647 Getting started with documentation
2be99cea8b501af5298351a09bdb8dfbc2162ef2 Getting started with documentation
c45fcf878116725d0fa7931e5f4c887471314159 Getting started with documentation
c24b53b3443a64c0fee782d000c63bf1e973f3ea Getting started with documentation
4d3da543a09a87a8d1abf94c5833357569ba2a70 Getting started with documentation
dacbfbaede8e4cfbdbaf71f16ff7a47f513b4bbc Setting up basic structure
e758786add2fc2493fa27d6cd6b1a851f9b871f1 Setting up basic structure
98e168f05b615a2aaa645814bb8d2a2886c79eab Setting up basic structure
950ee55af874ffcedc01d4676f8724322c319a04 Tweaking configuration
5c952aa91b938ed4b8b4c966fc7bb130afac3e67 Tweaking configuration
6268ba9ea54ab58193b225a64253a06551f2dc90 Tweaking configuration
1c5b3ea3e6af9cf23025f937c9d00adaf882e262 Tweaking docs configuration
c2eb2bb52eb06f78dd6449c92fb0ea4051d414f4 Documentation root moved from docs to doc folder
a137b4afdb7faca44e3e173addcb42e176351ed0 index.rst moved to folder docs
98885a805723a83a0aa7eba334ecf9634b09b601 Documentation started
1dbe28aff699dfdc49806e3021d89858c05ce13a Improve installation on mvc for asp.net 4.5.2
21100bcbbfba88703ea9ec5ec2541cdc26f4652a Assembly version 2.7.1
ecdac73a5112ed2abb85d84afdfcb047798abaf3 Removed debug text from template
8caae2816218e351aec77367255608b77537d9f6 Improved output cache invalidation modes (site, sitesection, page, ignorechanges, all)
76b6024a307b079af26edbf2ea7f61049b2e607f Improved messages
9a435e4aaf1387ffbebee42a322b3cc63b220a26 Text tweak
e147f92589059596f7668cc04ff0d354ec3365b1 Text tweak
3c3407687444d197be35f33c9b21fa532318e7f3 Package tinkering and adapting to new dependency versions
f56642fb020fc728983df8354a2cf122b3c5f8fd Changed .net version to 4.5 for framework assemblies to improve installation experience
c6e05f91ff0839292c521ab120cc052b27d8e59c Fixed unit tests
87c4619e328c166aef096d28b3ae3b0593ee557d Fixes to messages
63a48ea9e2d18cce3e65d4d47113f2f4ac8526a3 Allow html in messages and other improvements
27c719afab7254e1bd279043350dcc09d6fdf081 Fixed tabs on site settings
dc34a89bc7bf2e9d9b92e545cb760985580d1d66 Updated .Net framework to 4.5.2
98637c8e02de6c1323693bab69c1fe19b8438361 Updated Shouldly and NUnit runner
e37a8ea482900673fd24e8e5613fe065e717b5dc Updated windsor to 3.3.0 and fixed some issues using windsor service container
c606ac36133db042b957e7c7145511aa1a93a1fd Updated NHibernate to 4.0.3.4000
fe2bf3d7bf012c451b8228ae293727d63dee46f7 Identity Core updated to 2.2.1
8d51f5c3bb190bef60d624cf99ca292d94ddaff1 Updating NUnit to 2.6.4
afedf73c50b2b14ae61d087d2dab59f3d810f165 Updated Castle.Core to 3.3
fd673ccd1be0c8ae6febdf9dc3f7dcf07ed170de Updating Mvc to 5.2.3.0
b64299e03955539f13eb4f1b37c2357174f6f147 Changing .NET runtime to 4.5.1
e44e9411ffb08f005b0b8e2b6ac8f3d2df700f42 Missing file
4014c8bb53395c39c6c70b3ba2fb5a9ad90a2d6f Assembly version 2.7
09a83aba737fc3ce044fdfc11452a1b78591e509 Assembly version 2.6.4
93783fd10a8e10efdd89314ab31e2b9a3e664446 Merge branch 'master' of github.com:n2cms/n2cms
0f1b940803fda6b9d1a8859435a1ac256e410066 Server communication problem notification in management UI
75f64e0f98839ef397b2f2e2e2202fce53928440 Put mesages in separate tab
587db36fd799ab12eecfce09a9652af9ae05be57 Delete message and simplified management using site settings
86831b1ee336675afa0ebc85242a6add4e95d797 Merge pull request #628 from CarlRaymond/CarlRaymond-patch-ScriptOptions
336399b0a335596ab9cf2d6d32e04ed1fc44cab7 Immediately save last dismissed
672cf52d474a5be63e43a3abb0e208ab1d9f8944 Morphed Notify.ashx into -> Ping.ashx and made it convey messages
cc72a7a6f05c1ea4edc62c53529a6b5e31762e6a Messages localization, auto-expand alerts
a25b9c96f4a06327f033b86a498caf4fe7cf02a3 Improved messaging: support for alert, clear, reload, etc.
8a7f464b37be2e60ebdb79014a8c243626f7ab49 Messaging preliminary UI
61ad30ba40c2e6501a6d57b64da48807c1798450 Esc toggles search in admin
7068247450dc58a2472b0dc9bb3fceebfdb2de73 Management flag and message infrastructure
dad25c188880745acc84b0e3228e07c6b0fa73b3 Fixed root page control panel including zones
25205683c4f684148475e2f785fe68b27cf9b26b Improved slug generator client code by SntsDev (fixes #631)
b587a3406c8c5810c948d1411278920f57201c03 Fixed multiple file upload (fixes #629)
d222b53faeec432f16ba273f8a5e8246434772b1 Control panel positionning
310f5b78dc371386619d9ada997947cec0ea82a9 Update ScriptOptions.cs
3b2fb021a2d7229a701170bb978aab542bc24407 Merge pull request #627 from EzyWebwerkstaden/verybycustom
a0b27924eced591537e0f7b976ba31307591f230 [FIX] switch to varybyheader , cause we got an error when trying to use varyByCustom, this fixes the problem with outputcache and multiple hostname sites also. with VaryByHeader=host
210e46ba6744c5aab3f47728000424c32fbef107 Possible fix to be able to use VaryByCustom
90d4345111358e3696621490e9947ca227b738e4 Fixes problem with editable summary not saving it's values (fixes #625)
848b714a7582397265ecaed2f7a737bb933dbe4d Fixed management issue caused by missing namespace
c71d41488cdc10c4f8ae96a4cdb5179ab06bc149 Merge pull request #620 from nimore/614_RestrictCardinalityOrdering
3a3ff83a48ddb772af18f461eb17322740d3c884 Included targeting UI in solution
32248cc3cecda6ef31618a76ddd000fec8f0bcb7 Merge branch 'master' into mobile
7a0ac940c4812764199cc95872d324459ef3ca53 Fix for stack overflow during initialization
23e603f047c5c6a9f7e05178d95ebd4f969f5a3b Fix preparedependencies for VS2013 only system
41747582dbc9000798c999a62c58531ad1e8fa0d Fixes #614 - Allow Pages with RestrictCardinality to be reordered.
daff355ea0642810271586177fdd86e5b12d821a Show parent page when clicking on zone after 'Show Parts' (fixes #598)
871c0e0a28acf59c36f66387ba0edb911d60d8f4 Changed so latest draft is edited when editing part from the navigation (fixes #600)
83f69ce6983f760f2e2693887aaa1b3387a4ed99 Merge pull request #608 from bherila/master
9d745abff574cc04526ca7998efb944f1d563d08 Added warning to web.config transform
7dc351734a5bd1d509ee9b3d70d7e5cc3b412331 Correction to web.config.transform for LocalResources Nuget package (Resolves #607)
4a337b75f94da6823989bafdcd8c073a065a81b7 InstallationManager makes sure the start page and root node aren't trash -- Fixes #583
5d723ae23aa3baddb02f4b2b64ecaccc2104716b Removed brace causing compile error
3153085549d194ae8fa985c13f11b5863e76cdc6 Merge branch 'master' of github.com:n2cms/n2cms
7c596a95c4203e2474bb12c96c936afc4599959a Merge pull request #601 from bherila/master
dc9fc6c8ed1f0ea58d57445224388efd043b0a04 Update jquery.n2expandableBox.js
458a9063c2fedc10c89b2d32d79cae826fa4ce81 Merge branch 'master' of github.com:bherila/n2cms
7bf7e9ab16c68f00e49bc45748c65707150a8de5 Check for invalid chars in uploaded file name - Resolves #579
eee6e442eb9f7ab2e1fbfc6533a1eb26404d5b27 Assembly version 2.6.3.8
871108286635bda327a63fa023db87391c960c92 Fixed problem saving parts from the form edit page (fixes #571)
7380723eef232cbdf8fc52642bc8cc23fc6c8de8 Added info to the install wizard about increasing max upload size
438dbbf5c30ac0bc94ce9215947fccfef4e5bb5f Assembly version 2.6.3.7
3f04d85d2b3c91e4cb30e6038a600d578ca489aa Fixed dropup, some margins and padding when using toolbar on top. At bottom by default
5d524f08e7a315c34ed3b995acfba4f5d47ccaff Fixed weird padding on logon menu link
88384cc752dda8a1596dafa2571fa5dfdf2fdedc Build dinamico demo content
ef2beae668d87a9117e518486e74bed9a5632c56 Merge pull request #594 from nimore/UnlockPasswordFix
fd7b976d437459607581d916a7378707f7a4afb3 Fix null reference check logic when unlocking a password.
7fa7a44704fd205f00953749d2559a18688de899 Incremement version
b5e961274f432a7d08adbb16053797305c31bbd3 Merge branch 'master' into mobile
0b16f35ea054a1dd3b1008f9e929ccf3f79e06fa Work in progress
78cecbb684ca6195cde3efbeaae62da4c4a390ce Updated xsd with remote search config option
fab0948c5e87eb3d839ce014eabfec695a25d2b7 Changed name replacement away from aring -> ae
19be06800804907bb69e79a22251cbad2e41f10a Merge branch 'master' into mobile
692d0e370ff7fe0a4d61f47957904b562cfcc4d6 Added web config transform to tageting
ab8ad7e8633353e126358c2dac7fa79cfdc3c8d1 Tweaked phone names
f9564cab1f5604ef66386b5ff704248699c200ce Fixed targeting menus after refactoring
ef76e7ffb12fb34a4dc3ae0dac4cdd143a68f5d8 Merge branch 'master' into mobile
1fa1a4ff67f875aaf3e6bcf7d9ac51f6e9489797 Targeting nuget package
8e16b52ba3e87c3501b0f6cb987ba3f886bf39ce Implemented target container which allows rendering parts for specific target only
8238dd46b2289a1e60e3b1c670598712a7c027c2 Refactored part rendering MvcAdapter->PartsAdapter ZoneHlpers moved from N2.Extensions.dll -> N2.dll, fixed problem with actions when organizing
0f9e8c9c73eb736640c687fb484f7cbeedeb9c37 Target preview functionality
0428164835ac65e047d19c92f04491e8e6694fb7 Allow targets preview via query string
c56f76f309e61d607eeace31c4c48531221de618 Set view preference client-side without full refresh and allow adding query strings to all preview urls
a17874ce55b020433a9aa5281a3a069724ba2914 Previewed device selection
9f84598bc4ca6db0f53d2e1f7ca66b4be2e4b653 Refactored selected item query juggling
1eb510c0403139080833aa9ec87253e3257fea6c Merge branch 'master' into mobile
5ad21721cc59347472657b079c5bf2edbd590476 MVC support for target paths
b94103bb9ce9445be3cfb5df843164d6914f93e3 Webforms support for target paths
1e68c49420a4538fde5042c88be9319eabe4a91b Refactored targeting
b35c564ed623b89ccc85ffd770e1eed61eb72ebe Removed tab scrollbar, widened summary editor, changed form layout and help (fixes #211)
198cfea19af1c5ccf48398791e27c31fcd47a9d0 Moved context menu definition and added security option
f0491cf633daa971c168d59ddc7ef9f05f95b615 Merge branch 'master' into mobile
f4037ea5d32528f743c70328f52472277c1948cc Organiezd project after merge
7988b9a5fb085edf7b53149708147dc44647b5cd Merged from master
cf126052d637bfde3f4121d72b19a94e73340741 Merge branch 'master' into mobile
674d3e1b915727ab2ce93a83ae3acd96386f92b6 Merge branch 'master' into mobile
0fcc91bd5dda240f02632e4fc04630d4a576f404 Fixed icons in targeting localization
6ea4dd792a94b5301e012e918fe42577a20e8b5d Targeting english localization
bf2ac6ad3fe85e46eb9d28f94df4db0fecfa11f8 Allow extending translations
a4fabc879772f951cfc4f5931d777287e74482c4 Targeting icon fix
a43ce99c5ba5db3e03f88cfa3dfca2e958896d50 Targeting configuration
c8d5024022cf6d8b46c91bd4a4cc97bd83986e01 Fixed js error
19247a6d2abd67c46035b5715191cb07dac2cdc1 Allow missing configuration sections
39600385f0e4fa99699964b6559b3071ec613e43 Tweaks
12bddb54f31fdee8f1a8bf73bffdb86bd8629bd0 Targetting module
55380a0713ba55adaf0a489bc6c93be9d6c8ae4b Management module discovery
a25c76ba6fd628ab2dd44bcdef67f077c8a3b6ab resize directive
2a11b85139ddf4684375c5311be2a36118e809e6 Merge branch 'master' into mobile
357e638b1836ae98458e726d8a749425f1ea43b6 Device menu idea
801d8fef64548baab68e98fe54a73641d3f233ed Enable divider on sub-menu
```


# 2.7

XML content storage. Many minor improvements and fixes.

```
6eb8e4cad7436f141ee4dd7cdd7c4e14503f4d69 Leverage Bootstrap to help with Editor layout
5d10db58e60bc376e96928c75d763f8563fb341b Some progress on improving resources extensions, introduces MvcDiv
5fe70867419e8c58bdaebaccda2e271bc9f332d7 Fixed ContentList (warning: breaking change to ContentList API)
51e5e0275c48e4189ebf6e5d493e2b6064d7103f Refactor things that were using the deprecated GetChildren()
7f26ba0a3b21db3ea0badf5ad83063fffd00c805 Add jquery to login.aspx which is required by Bootstrap3
4826b0e46a1d7546a055d5d800349d2f63f70a97 Assembly version 2.6.3.6
2fb236379682ebf2b31d6658afef610323c6bf6f Merge stash: CONFLICT in ContentList.cs
3abe83d46456517cb55a5b6bd997b655d4a9cd95 Merge branch 'master' of github.com:n2cms/n2cms
da76455bd4c9e0c31f9767fbce3d17960fbe3578 Obsolte GetChildren, introduced GetChildPagesUnfiltered and GetChildPartsUnfilterPartsUnfiltered
a5b97122782a8d030e1989fe80e1c48dd5eb02da Mvc -> 5.2.2, SQLite -> Sqlite.MSIL
3e49c4405c7d25e428aadea502219a038fbe60ff Move filtering logic to client side in ItemBridge.GetUsers - makes it less efficient but resolves #588
0e89b6f426e960318b49e955122af4d5a9e360bb Add support for dynamic Bootstrap css classes
3bf4cae9a46893bfa48303dd7ecdb33ca1dccccb Add support for bootstrap versions in n2 resources config
5e4fbcd5abd8307172c9ac074137754c3ec119c8 Merge branch 'master' of https://github.com/n2cms/n2cms
2cb5ce8d18ec2e47f10c5b1ca1af9ab7c9df1e1f Merge branch 'master' of github.com:n2cms/n2cms
65b13434ac94c8ab82671fe3c4c0eb12b4db3491 More friendly error messages when using 'Relation'
d66ca352c87b67ebec1a0faca4c27904f1587c19 New features for ImageAndText part
a28ad80fe42d2980eecc1193a0a68dd4acfbfcc1 Created master page for Installation folder
4ceb81455696adc9b0f61b5a2df7aa681004bc81 New options in Delimited Data Display to reflect additional Twitter Bootstrap functionality
2b802d7741b03fa32ba2ea70e566bd55ef56f6c6 Clearer, more verbose exceptions in ContentRelation
20719d5800987a21807728953718b6bd437d9e73 Improve csvimport by adding support for escape sequences
b566f570d5982ef40a1d497abbcd5c433da13d3e Merge branch 'master' of https://github.com/n2cms/n2cms
ef77534ffb2b094e31c9207c800dbb1f57d22cee Adds config option to move the edit toolbar to top or bottom - resolves #570 mostly but leaves some work to be desired
161ff14822edb4c5517f1cfed83b4a54fa3ea39f Merge pull request #581 from nimore/DatePickerBootstrap
8e311e36f813b69fd3bf19e745f7895b028e5654 Merge branch 'master' into DatePickerBootstrap
085fc6683b6f325db547630e6c6f7f07ec1ea861 Modify the DatePicker control to work with Bootstrap.
be5827032d9f1cd817bf515bc829cadb6179577e Split Dinamico nuget pakage into Dinamico and Dinamico.DemoContent
a6d136fc3b40bc80b3659d410e3b376dd827eb64 Merge branch 'master' of github.com:n2cms/n2cms
205ee7094b8f3566312e7a002fca725933370ada Changed h1,h2 font in management UI
99fedccfceae55202e249e0354892786d0e71277 Merge branch 'master' of https://github.com/n2cms/n2cms
e737c7781c67e9c0e1fd6cd9cfe0427f176ab557 Assembly version 2.6.3.5
8b727b580d0818f64674fb9a29928ae531196179 Merge pull request #578 from nimore/UserManagementFixes
630edf74e85af94a4f42cf69dc98a686e5753f9e Null the DataSourceID
4ff0695e6ae2dc7a8414b47d76eaf4d585f40928 Fixed editing a user. Changed new password text box to use masked input.
667d5f80fe1f1e8ed4ddafec25eaaa8e08863694 Merge branch 'master' of https://github.com/n2cms/n2cms
4c665a760c67d99181e710c4b7d5776a6ba7cb48 Ignore parts removed from code when handling previous versions
17303fc515a980448e2466b3758b1cdfd1763a77 Made Recent Activity feed more resilient to invalid versions (whole page doesn't crash but maybe could use a redirect to fix.aspx)
51388570e7a890d55144f1be551f55d877f6a037 Added bunch of null checks to versioning APIs
af9fc1b7e7c3c7847b50bd3a99281a374431b32e Fixed buttons on directory/file
1c48be5bec35085dfaf15f1f0911103c30780986 Fixed buttons on directory
1eeb1bf05dee83923eed4a5ce4b813892264a53d Assembly version 2.6.3.4
799ed99ba315040d20996fffac9e6ae8c071422c Fixed more potential viewstate issues
e70bd89a95a0495b8a018c0396b3cbd790fadf06 Fixed failure to load viewstate when installing
1ef51aa6e15ee6dd93ef023fe005a5e56e7e6673 Fixed some installation issues (mongo + validation)
4e3f630e6adbe39c82610fd4554db9cc4c75c0bb Made bottom toolbar more prominent
84423541fb7a5d9aebdf550f8ed8a40d4890394f Merge branch 'master' of github.com:n2cms/n2cms
a3adf054018c01d11a77bce4522bc0bbfc6f72da Fixed future publish (fixes #574)
524c1401d8b3c359b4ec7d173bd55064a05e7e38 Resolves #572 - if url contains escaped_fragment, don't do a url rewrite
bce31de53ac70c98cb23f8f3a7d2b5030ab157ef Resolve #569 - improve error handling in FixClass.aspx
76812c14c2c6d42b54b0169785557d05ac8fbbc6 Prevent a start-up issue if the controllers for certain preinstalled parts aren't available
22a8aa6e2f644d57c850986da845cc1af4e93191 Merge branch 'ToniMontana-feature-mongo-file-system'
01c9f383764e2502b2b5d7d7bcc8997d207176ae Merge branch 'feature-mongo-file-system' of git://github.com/ToniMontana/n2cms into ToniMontana-feature-mongo-file-system
47229a5e4b87679c4639ca08e0b028460cfe5a56 Fixed compile issue with non-assignable client id and framed-navbar
0002522f8810071ba0a677d60f4af97b3ffed16c Missing mongodb project update
3c33d700c0bd47b2955f712aa914909ba1536c96 Merge branch 'master' of github.com:n2cms/n2cms
c3965403d73fec8bb668a97255262135876f274f Fixed some issues related to mongodb serialization and __interceptors
ceac355849e348f266e054fe6aa8f55756ee76e1 Assembly version 2.6.3.2
7132c8e373eaf07193798f493702ce7b645fa6ce Merge pull request #566 from ToniMontana/hotfix-directory-bug
43d0e2debc3ef46c023aa4b0c84be03dd1d611ad Merge pull request #567 from ToniMontana/hotfix-content-list
4cd54501ba077f000b2cb06f803820747ff05005 [FIX] Index may not be less than 0
f4b7ceaa19e96b144b48a1b8b060d9f9813828ed [FIX] Directory: AddTo() needs to call ClearUrl()
ad7b6060a73baa40f5ae7a9b16fce8bd4544592a Resolves #35545 The Controls collection cannot be modified because the control contains code blocks (i.e. <% ... %>).
b940413dabc9745a093bedd270df32ee7dbf765f Merge pull request #563 from bherila/repackaged
a0d2eb42397c5b56b86cd75830780c4fbccf6f40 2.6.3.3 version into Setup script
63be8a871b11eeb35a0290168a632bba8f87f487 Merge branch 'repackaged' of https://github.com/bherila/n2cms into repackaged
576356682a6ac1f102cbab1390d3f4b997f6234f Removed a few more hard links to resources
96ba5a839d5a4ecb21818156f96f60fa788f8c5b MongoDB: MongoFileHttpHandler - Http cache max-age bug fix
2c58c1fb55c68818ef7f1305e33fc18315be5361 MongoDB: MongoFileSystem - An implementation of IFileSystem for MongoDB
826cf7b1c1f26cab41216fafb1de5c2b4eb52f4a MongoDB: Read preference and write concern suitable for a replica set
eb10b4bab27a58d83633daa9d24e06edbe4a9b5c MongoDB: Ignoring extra elements on mapped classes (I had problems with this using MongoDB)
b9d5724df6d5c3898a173dc9a340ad3c50429656 Merge branch 'repackaged' of https://github.com/bherila/n2cms into repackaged
25adec70a9b44c8c5ab00f3ab6f27b228a74945c Merge branch 'master' into repackaged
15b55684893c44369d54a18b6a30d0aee7ebe992 Refactor libraries to use separate Nuget packages, and add N2.Resources meta-package to pull them in and set up proper configuration.
4013d2976aba317262dede4f8cd672387321fcb3 Increment assembly version.
fe8e81014114f141b8a759693c2de3200f312c3f Added some new resource mappings and removed font-awesome.
db6ef7bc6c403966503ff82cafc98106949ae4ab Merge branch 'master' of github.com:bherila/n2cms
e137c0689e5af494726d2a0d55f6e272620a3106 Fix wrong font URL Discussed at https://n2cms.codeplex.com/discussions/564398
93ae6a8e8ff55288c617367b70e582fcad8ea328 Should resolve #529
38ba7bf00ad28a4fc9a6749e0dca9dc6f053b744 Merge branch 'master' into xmlrepository
f6ef05eb45f3b5d755f642c32d45ebc01faea25d Merge branch 'master' of https://github.com/n2cms/n2cms
e2edfb2c65963e6fa53e96f6db2756a3993b1fee Merge branch 'xmlrepository' of https://github.com/bherila/n2cms into xmlrepository
4e27ba425abfd8b07abbcce025030429f181cff5 Merge branch 'master' into xmlrepository
44c11938fcf8630e03e708af0e8737c4177cd5ec More exception handling
540b774a9f5267fc552ee97275232252762683de Merge branch 'master' of github.com:n2cms/n2cms
667dbec5d8a153d8a76dc3128d7e913e134da6d9 Fixed problem with descendant ancestral trail not beeing updated (fixes #545)
916104b7fa4c963c55b06c88b1d5666ac6d8bfeb (Progress on #557) Fix build break and increment version number
3ed6cda5b7fc6e16a1e8f10af56b2e2871dc3a22 Merge branch 'master' of https://github.com/n2cms/n2cms
fdbfc793f3a8a913260401a331e4b119269c608c Merge branch 'master' of https://github.com/bherila/n2cms
0797e52eaad920bb91bdc3549b17853e543926d6 Working on #557, making importing more reliable
85077cc407c9ddc7ead1587d34a749c6e233ee99 Merge pull request #556 from n2cms/NoJQueryOption
245e72223aac87502f22cc70f62c484e1743c545 Use double quotes in HTML strings in SlidingCurtain
962a097fb9befc5567561bc5665f1271b5718e49 Add option to SlidingCurtain to avoid loading JQuery - Resolves #554
fa4803acdbc696f4a99d84fefe062e0e2e5b69db Merge branch 'xmlrepository' of github.com:n2cms/n2cms into xmlrepository
95cb5a3c2d59dbd6d8f8d9b97722463451fb0cc8 Adding on blur to the invokeUpdateName
f5763337115afc8542885b0178d51afb2e41563a [FIX] Added preview and save if Save and publish
1f2778e372f26d0e8d2c018dca6094212bfe7868 Merge branch 'stweb-master'
2609da76664d5f316c2cebcf974fbaeee59182e9 Merge branch 'master' of https://github.com/stweb/n2cms into stweb-master
62166f2a69e8da65225aeaccada2f62e53c2f33f Merge fix for admin changing user password; Resolves #555
66904d2d88eb72fdccb915abf474a1e52638a7f7 Fixed problem with reindex-all reindexing non-indexable pages
e681c0f20e443d78ac333665ef004e826e27b61b separate solution for CI builds
a406bc29bba2698b1a555960f286a65ad0552179 moved code from Extensions to N2 so that Razor doesn't have to reference Extensions
2b34293a2a2036d1ea639834e2d02994bd4d629d fixes & improvements
5466144fb9bd6906e8d928aec1005a0c537912c6 fix cloning
08cb3448e4aed3f547e744e4f195de9d86dcbf98 fix UI bug with disabled roles
899c53c79b7df9af58bfe322141a072d43a47813 added validate scripts missing
971b4f022f397311c87b2905f5fad259c3de24bb packages should not be checked in
7932432d08271f2b257f2d8902814f8532014d59 Merge pull request #550 from EzyWebwerkstaden/master
5f13146a4a448f14a3a19539aeee8d4991adca2a [FIX] If a page doesnt have a language it returns null in the inner.GetLanguage
66b3c7f714350e1ada7f237aa5a1bc1c5876e62d Prettified create user screen
a6c481b92a9551ace3c331cac7a9cd26f924cc65 Excluded missing file, added nuget refeference to microsoft.aspnet.identiy.core
317656603f92af2ef24f04236446b55af7c7c64d Merge pull request #548 from janpub/master
cbfb10f436c66cf7e21d1ae75e431403aeeb39d7 Merge pull request #541 from n2cms/proposed-upgradeversionfix
3234a28a7572dcdb15c6f32282f0010b68f4f115 Fixed adding parts to zones within parts
e713427b3b77168f2790bcf961f7f51e87308301 JH 4.1 - TODO list
e299c2488e681bad6bcfec9f5b1ec36be7012c0e JH 4 - N2CMS Dinamico on ASP.NET Identity.  To run DinamicoIdentity you should add N2 and Dinamico subfolders. See also: NamespaceDoc (N2.Security.AspNet.Identity)
c51f6cf15ec08dc12aa7956707c21a27accb501f JH 3.7 - Dinamico PermissionDenied handler redirect to custom or default login page
4172d8730334c6264ac9cb3ae245f50088224662 JH 3.6 - Dinamico LogOn menu gets link to "change password" page
5efb1463d519e461381106065f011e6e4cc88127 JH 3.5 - Fixing StartPageController: AccoutManager wrapps calls to System.Web.Roles
88f9a0660a66afd7a7cbeb05b39ab843c16dcac9 JH 3.4 - EditableRolesAttribute is moved from Framewrok to Management project
40dd0b93a9b06cd3046ab6b616941c2cdf22d437 JH 3.3 - AccountManager: allows to replace membership subsystem. MembershipAccountManager: default classic-membership implementation. All calls to classic-membership API are replaces, e.g. System.Web.Security.Membership.FindUsersByName(..) is replaces with AccountManager.FindUserByName(..)
ed260ca4868714cf7d9d8679c6e4b1d692a2b8ef JH 3.2 - AccountResources: allows to replace N2 Management user management pages with custom implementations. MembershipAccountResources: default implementation.   All references to management pages are replaces with token expressions, e.g. "{Account.ManageUser.PageUrl}".ResolveUrlTokens().
4a95985c3309f9976e7c2bcd2935ab4f29033eee JH 3.1 ItemBridge gets additional methods to centralize logics (user roles management, IsAdmin) and to support upcoming Identity.
48577b58f2de37b0e478c56c605fa7968763ef2f Added parts count to version list
a07bf3f2653372b81dad3d4d157646a7e495d384 Fixed issue when save/publish is disabled
170700085b8ff8c5aca6750b41c91d85d783632d Stay on site settings when saving
fa7a248294f8d977bd970f961564be2a396d6962 Merge pull request #546 from bherila/master
beb49bdb69453582a6dbdd8291d16e269704c634 Fixed future publish dialog
1fb6094c174660dd44834abc4a26ec072f3c5eaa .gitignore cleanup
115dfb5733aa7f04450701bd2d59e319c48b0c9f Removed xml repository from repo
d2503956e38f787af24c7e59adf493af27dc063a Workaround for possible risk using GetUser
8fd541492e6623026c8fd59fe960afcd4f0c4dd9 Merge pull request #544 from janpub/master
1106eade3735c5bf7c95f6b64ebf860492e8082b Merge branch 'master' of github.com:bherila/n2cms
029533b376daadba64cc8440a2820c4bca4fc3ef Fix #547, replaces href # with javascript:void(0) to prevent scrolling to top on ContentBranch
22730d548998021583c4b9a66b1597e118df059e Small amount of resharper refactoring
fc17cb814d9de5f29933e988beb303a6dcdc22fd Fix bug where web.config gets empty host element added while attempting to remove the vpp zip config item.
fd6d8c95405638f389e470c570cf92ae366040b8 Fix spelling errors
8a2b15b21893fd87ecf7d437ee4b167a0ccfff5a Tabs
608878e0c2d6d0ddb9c798627708f8d72c533a51 Clean up permission panel a bit
bfd40a8299d04007ebec6e8d46a8f378266ec4b8 Resolve #542
708b5ba216677c0aa17057e2b9acf9e2739d3e02 JH 2.b - typo
7d94aea4faaf3b83418450818272caf03e98cfd4 JH 2.a - ContentMembershipProvider bugfix
b8cbcec08e0a10a815538c689d76cfd9bb7c3545 JH 2.1 ItemBridge and User type
c8d37ce740f489f8de0ff65e1bd1873452128149 JH 2 - Basic N2 User subsystem extensions to support Identity
4ca51388e611f81d4a803304757b2c3505b659a9 Merge branch 'master' into xmlrepository
456f5b817063f62c796a536acf6a4417c5a108e1 Merge pull request #543 from janpub/master
7596fdf39c70be46ba186eaa30381a3735608111 JH 1 - Basic code enhancements: Token values may contain tokens
794083147bc3ee3cb4c439553d0365941c5faef8 Proposed fix to UpgradeVersion http handler
269d833e266f3f0633511e39a039daa0a4d6ad10 Fix misspelled ConventionTemplateAttribute::otherTemplateName property
f85afcd21d611fbf8c4eb47a507947dbf4e66e72 Resharper suggestion
0c967119e058ff06e0aac707ca3b5a482cf7f2e4 wip
c80dccf55b9c3d65c974e52070fcb8296374e115 Further optimize migrations
c92357c80d2d89d041a3c978e855bd6b670b54b3 Merge branch 'master' into xmlrepository
cdc49d6ed96086db356ab7ccf5ca04299186c527 Changed Links and Image migrations to check applicability faster
8b83a6dadcff5d455d22021637cc62e27cee096e Merge pull request #537 from bherila/xmlrepository
2b529bfa314a0989d536dfcd104243e384796d41 Make upgrade asynchronous
b5b36fac7a6e9e2385f71f70aa2353c23493eb3e Merge branch 'master' into xmlrepository
42eb44257030dbb2c589e06d526f9bce5d5740db Merge branch 'master' into xmlrepository
f09a19a8770cf12109552c656a8b508f2a5744db Merge branch 'xmlrepository' of github.com:bherila/n2cms into xmlrepository
cdbb720458a195240aed1997e6bf0fb6c4cc2644 Added some missing semicolons
e2f5ba360ae37acacce66e2fa07fb533ec7df9c5 Merge branch 'master' of https://github.com/n2cms/n2cms
1a3da36bb726ad2613a1bf4cd3121fb709f0c001 Updated font awsome to 4.1, moved toolbar to bottom + various tweaks here and there
3ebd4237cfb385733636732bad5e7b0dc71d0f63 Tweaked visible menus
bc9e4939f0f927c153537cb06c3ed20e0173fdfd Fixed sliding curtain when adding parts
44feec6707f685392f92dd9cc1d65ce0377a9f39 Updated xsd regarding angular path
8da30bafec7b4b31c92c51f85fbf0936c698a33c Updated project file
52b37fabd722074989ad82e53ea9b471ff3abb28 Added ckeditor and angular back to the repo
873cec9233794dead915f2cc072fbac99b39863b Merge pull request #533 from bussemac/master
541bf56b1630ea8690ac49ef32e0826944f28e26 Merge pull request #527 from bherila/master
2235079845098fc793e606ab9dcbecb456e7e36f Adds tooltips to EditRecursive. Fixes #521 (upstream)
76aa61f271563aaa9e81203a03edf91aced91934 Merge branch 'master' of github.com:n2cms/n2cms
ee42c914f58265e66fa2d3658b49f2c7f3cb13d0 Assembly version 2.6.3.1
d093b9126714be275186921390e26f7dfd3d1ca3 Fixes #509 by adding Organize Parts to the context menu of each page
a8e3c4ca79ebbe40a5824d3fb8ec40feca745517 Resolves #431
e4973a17252343c7a7042f98089894c36f0992c6 Merge pull request #524 from bherila/angular-to-cdnjs
cdd83dc11a1aef932a25316df10bffd39e87cdcb Merge pull request #523 from bherila/ckeditor-to-cdnjs
4a09e7a939775ddb3a425f08cfd49510fb156a1f Merge branch 'xmlrepository' of https://github.com/bherila/n2cms into xmlrepository
b3d9adac003ee55109b64af1a912ccfed58874b9 Resolves #431
c0e32f99dd66db49f82b286ad224e9fc6005ce56 Fixes #509 by adding Organize Parts to the context menu of each page
5eb4d43b1f6c2221bf318626bb09e9da963f2433 Default HelpText title to builder.PropertyName when title is null... resolves #520
3ec54d26b697253bdecea2bb45ef45cfb7cb64ec Merge branch 'master' into xmlrepository
d6237728c23c482c9ed139ec20dac243667beb4e Don't throw unhandled exception when access denied
63e6eaaedf48168b2a8cceeeaa8e249165a8f809 Move angular js to cdnjs, per n2cms/n2cms#519
2b1009e52d736ca024dda51c47dc608767712b80 Move ckeditor to cdnjs by default
4e26f0d32eccd161ef86b42e142e2aa91c77b454 Fixed child ordering in xml repository
29129a05893593b5c12efefe398096fd92aaf8b7 Cleaned up some unused templates
15f379ab255a9380e05c9320d0e27a60dbcb9219 Fixed issue with xml repository overwriting ids
d42cca2cb11a4e70f0ff5f800f0f492979925e3b Increased size of control panel opener/closer
14ec16c0b8f0e076e1d872a7ef3f14f1554d48cc Immediately save same user settings to avoid confusion (fixes #508)
c613157e10626381604b44e7d2604b75fca5d2f6 Fixed FindUsersByEmail (fixes #510)
0dd20dc5baa294f02bb2e665e502d30fa650e876 Merge branch 'master' of https://github.com/n2cms/n2cms
8d89005d4cbe74687dd8d6fdbd042d1b0b21e748 Merge pull request #503 from ystark/master
f4051ce4cd0d614361dddaadafb3ec6d25aca4e6 Merge pull request #511 from lundbeck/editor-titles
766d0e97372d4c5fcdbe58f220f4c74f06da2076 Replaced enum search config by open-ended string
75157342b28585c901a1b8cf2eab0a91057b17e2 Display definition title when content item has no title.
7fa221991e511d3ba6e8d8c22141f42d907a9790 Fixed issue with clashing xml repository ids causing unintended overwrites (fixes #507)
da9569aace267738ae7dbefe7a93a39c601622e1 Tests and improvements related to text search with xml repository without lucene
4e335649ced8b692fa2a1102e270eef0a539c2c3 Updated config xsd with xml flavor
c9f252f468951b1471d23fa2d50eb99bf7e0bd8c Assembly version 2.6.3
48c0cd2f245a2441b546d2d859d4442a1a434499 Installation/diagnose related improvements to xml repository
dd34d019242f6e89cb675a134c2fcd7b7d6722a2 Merge branch 'xmlrepository' of https://github.com/n2cms/n2cms into xmlrepository
9fcb7416bf6aecaa2b92cbaf4595deb0019b3182 Refactored xml repository file system access into separate class
70a5ee76e5b1bc578041977b993e2984e7216594 Minor installation improvements
9f02e8c5fdf16420f56e14437aa28d2ac1f777f7 Removed SafeParent and ifdeffed some traces of safe url handling
a1f3e34c28623972b7a5e5448fdb6b3251b459c3 Fixed parenthesis for union lucene queries
c82087b436c454f8f1d5003259bb1a8e0860bddf Merge branch 'xmlrepository' of https://github.com/n2cms/n2cms into xmlrepository
c25ff735c1fcfff5f3c9c63ccd429b8b0925cd91 Fixed versioning
2a370a77b9e10ad2df6b3e352937606384c5c796 Fixed problem using linq queries with xml repository (fixes #504)
4d2481c509291958a7a645e566d3583e84ffc7c9 Replaced null reference exception for missing connection string (fixes #505)
72a4140d892201794500bf2e4d0cf6fdde327ae0 Changed some things back to resolve some unit test failures
192f6d783bb98c179635b5ccc0f5be1b2f244772 Merge branch 'xmlrepository' of https://github.com/n2cms/n2cms into xmlrepository
16bb78693e761e343f35b834606d40e1f5bd542c Fixed problem with swapped day and month in DateRange control in certain cultures
ffe905b571cce152fe3d6653283dedc38a877d12 Refactored ContentVersion to support xml serialization
64e1167437bd0ca7cfa00e570a2337ce4c368bd0 First stab at serializing versions
4b6cc96db47236ae7430a9c7ac58d1f342392b44 Reduce risk of concurrency issues when using xml repository
b9be49b2dc32191aad66eba3f956a0cc6920cb65 Merge branch 'master' into xmlrepository
139a2613cf5af216bed4f78ba13121530fd36ba3 Improvements to the xml repository
5cb835c7b5fb8b7b6e5d43af735fc5f38c572d26 Improved equality and hash code
abeb9fbf55523fff4640a1edd36135d3a53cb1be Xml repository support for children
5df05a44fc18904e5e2331fd8c2bd2d90871fc7f Merge branch 'master' of https://github.com/n2cms/n2cms
35b4518e6f07b02f1b87a9e7ef515862286ec1db Made site startable with xml repository
04f1a6ce84311561c81576c1a7d6deac968ac982 Merge branch 'master' of https://github.com/n2cms/n2cms
feb3e900933e04e9aa6a76f6f0fbbf3cf2e8f9c0 Configurable xml repository location
2757b5b4f667fde050802e7dde66199330ace435 More complete xml repository
d294d9d4e77cd129f4a851963a192aabb9151c34 Improved xml repository
f50eb1166c7fdfd3d99695af66f1fea93465ac77 Exression -> expression (spelling mistake)
c6b92c99a9e5e2852e703abdcbbeb90cb384b0f8 Merge branch 'master' of https://github.com/n2cms/n2cms
a98dcf57f4eca54895afe0b56b4b8a2fa02727d5 Items drag'n'dropped in/out of trash gets it's state updated (fixes #371)
375945e533297b2ab5a9699524b5c5047d655aa5 Fixed some build scripts
475887a6905b267bae541026d7774485e8173b92 Allow parts with dynamically discovered zones (fixes #489)
10401b8ba35a1c9bd8c91241999c790d03a669e2 Fixed resize (fixes #478)
b615759624c822433b1993d502205fd1f09b0cb9 Assembly version 2.6.2.2
4567badcf1f5fd0d9f431d702e30fb911912fa84 Auto-expand to nodes when navigating in frame
a621d949bc918c82325aefadbf26660911d637c9 Merge branch 'master' of https://github.com/n2cms/n2cms
629746dd69e126de3b43581832ac9d1ed44d4512 Fixed unit test
98bb3e57313dcc0945982e087e67b248c1010eec Fixed some machine-culture dependant tests
dec7fc39fcd112b3846ab49adcda233b59f80300 Test fixes related to n2versionIndex and slug changes
6832d29eccb14ff7fd061f9445dc894aed2cf6d2 Merge branch 'master' of https://github.com/n2cms/n2cms
1ec16418c821f6575d663665da7aa65e95bfa304 Fixed some unit tests related to n2versionIndex
dbd74eb4193c9057a66d7c805cd72b7374553949 Merge branch 'master' of https://github.com/bherila/n2cms
f14f88fe4d403731681b9fab88f2a482d3b9fc05 Finished (hopefully) propagating page->n2page, item->n2item, etc. throughout and fixed unit tests
17cf260ba102bd1be26421437803df76029f7903 Updated readme
3eaaf5b4db02eef479348e497789a73d4696af50 Updated readme
4c166e7b8e3de3c91a96000d093165858ba2d498 Updated readme
8ff8e1d6dceb1922f877b479c0ee00b5f943a360 Updated readme
bd8c810b2d61791a807698af8d18c15faffa2958 Fixed some unit tests (by refactoring ISafeContentRenderer service -> static HtmlSanitizer)
24aff8d8e709be57bfca51c112a7c85f595ca892 Nunit 2.6.3 on some projects
52a0cf40d9b1c47ba76bbde3af96e2f6459f60f1 Merge branch 'master' of https://github.com/n2cms/n2cms
35d8b1813cabd3e2481438e68fc645571da6ecab Fixed problem with not displayed publish/expiry information (fixes #494)
741d0366f67b22a6da7e1d82e160b13eafb3cecc Merge branch 'master' of https://github.com/bherila/n2cms
2d4f6727bd6197a53b95d6d02781c4ff85c4407f Merge branch 'master' of https://github.com/n2cms/n2cms
ec0eeed72aa87008550fda9a40838bd8807c793a Merge pull request #9 from n2cms/master
0f7d0ac91539fbb7749ce35abe6167023498cef1 Check filename blacklist on EditableFileUpload and Tree.aspx, allow exe, disallow php
0900798c0e2a2d2045d069a347a290aafbb56357 Configurable blacklist/whitelist for uploaded files + some fixes
fc1ed08e0d9e337759781acdba9d60aaabd2ceee Workaround issue with string type on discriminators and NH4
9095e450e27b83c3e40d286c3297c20d24ee2b90 Removed unused web api config for mvc example
b9512eadcdee674d98d898f2c171266d3cb7d8a8 Made forcing localhost urls optional
dcad57e62bef600cc3e4f53bce0d0ee94132764e Bumped config version
b307975dc07c7bb79108050f7bcebc8f5ba7114e Fixed nozip upgrade with configSource
619dcdbe27bbffc8a526fac4664704edc587d588 Improved config changes during nuget install
357eacac216b615b653b461c712168a95b1ebe32 Consolidated sqlite references to 1.0.90.0
7010edc292231b1f88248ab42cd42e5d1039e952 Some nuget install fixes
2c0cc7289fa4daeecc827127818ba6d43991a4e2 Removed firebird from config xsd
f5e53306c9d65f029a1010a23eded440d665abb4 Remove support for Firebird in anticipation of NHibernate 4.0 (Resolves #480)
dde2a444298b271e3913a4f22a112dab89f32893 Added english text for users without localization (fixes #476)
fd4d8670970ed3e75acf82f65c2d8a22707a356f Check filename blacklist on EditableFileUpload and Tree.aspx, allow exe, disallow php
65711a424f354a76cd022916f3c9ffa3a5eb75b2 Configurable blacklist/whitelist for uploaded files + some fixes
967a798d93b9237294adeaed87dfb8abab0a3d4b Add 'php' to list of disallowed file extensions
11a22bb9e47b9bf8f925a5bb0ff026b81d49ca28 Merge pull request #487 from bherila/master
27ab4b0bbcffb679c4d9c38e55a57f691f3dafc2 UploadFile.ashx no longer allows unsafe filenames (Resolves #492, use ?w=1 to view diff)
9bb6f1aebaee3bcab70cb871f5ac381d20a96ab2 Workaround issue with string type on discriminators and NH4
96171031ba021f14b0e29c0889c3f3b072ad17fc Merge branch 'master' of https://github.com/bherila/n2cms
5066fb49603f1a81a69824250fd8557de03aa7ae Removed unused web api config for mvc example
29d004196c1f794995c6f7d92cae11811691348f Made forcing localhost urls optional
df9936858d391a4fc48ba6788fb2b6ec0c1bd600 Bumped config version
ad346f3e4bd44f609cdff1affc8214abf1e4f99b Fixed nozip upgrade with configSource
7cbc1d9dff878aee5b88554e70e9d951b7028af8 Improved config changes during nuget install
a61ca437d1dd4e8c72bf71ad1b7dfa4e7a95a4db Consolidated sqlite references to 1.0.90.0
6b364cd35f6de6d4b1b370883fb8a7b9a09bd9c3 No need to force upgrade Razor to 3.1.1
ecd805671856092a6c12499392a4488e173bb4ff Some nuget install fixes
e935c5f4d66f2c908acb5433f0c3d5a368a87926 Removed firebird from config xsd
c7d5230b8b8d8874b396253ad68abab49d480db7 Remove support for Firebird in anticipation of NHibernate 4.0 (Resolves #480)
7e9877450ea7a4e305341a73d77248c9c1157eea Merge branch 'master-2.x'
6a0a4e82d820cc8ab348ff08017812c161dc5be2 Fixed some H1-6 -> Hn
b8ee32c75de0a5e4fa0c9aa6ac98a872de3693bc Merge branch 'restore-mvc4-5-support' of https://github.com/n2cms/n2cms into restore-mvc4-5-support
6de96bca40d705d71296c4bafc02bb52daea9ba1 Fixed upgrade sites using configSource
f35fb930bb11bf240529718eca35fa490ca95bd8 Merge branch 'master' into restore-mvc4-5-support
284ccd40d8c4637a2749b7716e2032c645642bd6 Replace n2:h4 (which no longer exists) with n2:hn
a36a3145b98aad6790ee13f18a2fc53fdd03ce2d Do not force sites already using MVC4 to upgrade to MVC5
d7a055e8eb4c1b697d9256e020e0b06df556de46 Fixes to support install dinamico-bundle on mvc4 and mvc5
f4893e611fc48bcd4dcac5f45868e8ba184d6b27 Merge remote-tracking branch 'vsonline/master'
c2a7c6b853c8311bd70ec122c00a978dfd1db369 Assembly version 2.6.1.2
2a2bcd7d81230b668d8540eb5636d76c6287e7b2 Replaced jquery.multiselect with chosen for multiple selection
e9a90de1ed25d4a37a8bd5839ee78d40c486a23e Merge branch 'master' of https://github.com/n2cms/n2cms
07402eefd0ba61a6b34919df51098ab04862b8d2 Removed default configured windsor container for mvc templates
23b642e648cc7e157812b27c46d4aba434036aa8 Renamed some views after deleted N2ModelViewUserControl (fixes problem starting mvc templates)
75e3cad901108a828f8e1373d7557b36b52584d7 Fixed typo
8538d1c86f021ad7521def9a22ed402edc75c0fd Assembly version 2.6.1.2
521243bd129366b3394404fcaab3d03b655d33be Changed config dependency to 2.5.2
8a446a9263da85e4c40b435f39a33b94f159a095 Fixed redirect IE8 issue (fixes #481)
87f303c52fed62980c606a93b1adef34dca49f2d Fixed redirect IE8 issue (fixes #481)
efb018fb74d0bc0d0a8a154ef053d293e330a409 Excluded missing files from csproj
463bbbc089740a9f42a3a272cb6965c243117604 Update build scripts
482cc94cb988cacd05b9af6bb5b391715953a4f2 Fixed problem accessing startpage
ef86e04e2cdc4912141d57b15b641983a0ad8cdc 2.x version
7f6633b00030f85aaf8dec5b32ccba290d987e75 Merge branch 'master' into master-2.x
c0cb8a68a8ba043c3560bd9188d727a169a3e3bc Fix bug in MenuPart where duplicate menu items are created (Resolves #472)
f19da2d93b418e1c5675c90d81f5951c6eaf1741 Merge commit '4452202d6b0e7479dcbb3d3cea2a5e250ff761b2' into master-mvc4
901bbc0108150950f1f9ad403bbf6e8bab571bf5 Merge commit '1c60a538f58ed69dd33c934bdf6157d4c9cad6aa' into master-mvc4
```


# 2.6

Rewritten management UI (blue toolbar). Client API for many operations. Mongo DB support. Deprecates .NET <= 4 Many minor improvements and fixes.

```
52ef099fad98171ef41f4794093ccc222d3d1b52 Assembly version 2.6.1.1
c19c32cb3ee8dfe4ab9d32703768c07c29ad64e2 Remove support for MVC5
b513d0139c8c225d09246fcef72be289c03aaf1f Remove tight binding to 4.0 version of MVC DLL
b9e041439a13d6dfe9b5bbdca2ddb24aa387551c Update dependencies on N2.Management packages
797cafcfe32b60cd40e1460bcbd004b14c580295 Update N2CMS.Config (try to fix bugs with config upgrades)
bb51d24f105b3e1676df22aef34ba66c6871beee Change version to 2.6.1.0
8bf778442e199ea484843b1e1bba399f117e588d Merge branch 'master' of github.com:n2cms/n2cms
9671f626f59ba05aefc1dc33396c21f74942f6a7 Excluded missing files from csproj
e4ec7db3b22cbbef787f80e48c6cd6f7491d3e34 Merge branch 'master' of https://github.com/n2cms/n2cms
fbfd1931cb13ac7855cfcda8f93075fadc90feea Fixed redirect IE8 issue (fixes #481)
a7c8f60238224bdeed9e13fe17255f3f6acf5392 Update build scripts
4c7ba3d93c8e30b9fe81e3d092cf87d2040e993f Add tagging capability to Dinamico news page
e8e1d911e18d1687b406ea43dda446c949dbfec1 Refactor GroupChildrenAttribute to support #322
5a354a8ad52c84c941387bd34645781256037aaa Resharper EditableRegistrationExtensions (ignore whitespace to view diff)
6083bc2fc73985a777a0f41cc8bdef77a9a753bb Remove more obsolete code
b210e9dae814b800036d4720f55e5f30cda37d97 Fixed problem accessing startpage
059e1143f783a2948035f56165df0ab15a930a8c Add tagging capability to Dinamico news page
0411fae479b187633201312d77750b77ba0ecc2f Refactor GroupChildrenAttribute to support #322
1a06b6e4c0adcf3c759b0101ee450f3faa727d50 Resharper EditableRegistrationExtensions (ignore whitespace to view diff)
730a388ff7ea6e5d583acbb8974a56667f7865a3 Remove more obsolete code
48ad441e50f29d27eece3b98150818e5bc27b4bd Add ReadOnly property to EditableImageAttribute, resolves #94
928085cbc49da6e3236cdfee532dc9271840d921 Add capability for IconClass in template first definitions
44c9e4e804b7c1c003694fd639773bb934bb715c Change default query keys, resolves #462
1405f4a99cce9927aab1524779f551606aa55d2f Updated docs
76553d9622887ec7789b6eecd02da5874862adca Create branches.md
4357f43d1b36cab47d44a78d9d9a14bbabfdd978 Revert "Fix AssemblyVersion to 3.0.0.1"
324bafe5dc75468a0790d20ff14fbfe7ee3e53e5 Revert "Updated nuspec files"
19b96f523889857b21c07a8222b9a5018041f409 Revert "Now using .NET 4.5, latest Razor, and MVC5. Windows Server 2003 no longer supported."
b90a2f8056a7bffea7578793acf3cd2f96ce53cc Some code cleanup (whitespace and such)
58bd45d0e2ce727dc42733318fd14efd9eccc10c Fix bug in MenuPart where duplicate menu items are created (Resolves #472)
d7481a4488452339c6e923ead9ca122335852a01 Fix AssemblyVersion to 3.0.0.1
1a61cfc53c6d153869e46478e2c63e4bd798b8f0 Merge branch 'master' of github.com:n2cms/n2cms
b7306a14233e3a0231c145dbd26759963796e34f Removed obsolete code (possibly breaking change)
88b4eeddc7be82ecb6b5d4ea6bc358726f26a1d6 Merge branch 'master' of https://github.com/n2cms/n2cms
44151b627638b75d787a1b4518aa3e1d7fd8ba43 Updated nuspec files
070ce8765d68c88d0b72ba322dba9735480e7c27 Now using .NET 4.5, latest Razor, and MVC5. Windows Server 2003 no longer supported.
4452202d6b0e7479dcbb3d3cea2a5e250ff761b2 Moved search to menu
1c60a538f58ed69dd33c934bdf6157d4c9cad6aa Merge branch '2.5.17.2'
21cacd822adaa8332740e19725ac793094071c11 Merge pull request #468 from lundbeck/dragdrop
ca2605f82aa2a152e6d4259647bf99a778e7beae Order list of parts available for drag/drop in "organize parts" mode by sort order first, then alphabetically by title.
baaa6b938401f484bc6ca4f0378afe060eaa4375 Filter the list of parts which can be dragged/dropped in "organize parts" mode to only include those which are allowed in one or more zones within the edited item or child parts.
ec25159836f3794e1032bc378416beaca44a7eef Assembly versio 2.5.17.6
c3290e7daa8ec711764a21f938a33eb675faadf2 More logging to security enforcer
6eed63d0f10c0c7860441b1359b697ffb2f9051b Reverted trailing slash from 'safe url handling' which cause changed urls for some users (for trailing slash configure extension='C:/Program Files (x86)/Git'
550dd72241cb9a24c924ae9523f7cbadbb89d0a2 Assembly version 2.5.17.2
35d90407d6c106a0f237558c629dd10a02090556 Merge tag '2.5.17.2'
6112f0b41dcc83824852e045c39e2aa48813bb68 Fixed issue cleaning trash when using AspNetWindowsTokenRoleProvider
6e2242cc743d913b36040967b8ff3c78345af20e Merge branch 'dinamico-responsive'
bc81a01cf9b9888be2f38fab286f43ac4f9dc23c Fixed nuget install problem with forms and validation web.config elements, assembly version 2.5.17.1
cc8942bdb9b54960e1f90cffbe1e116731c82796 Assembly version 2.5.17
e638b919d548f3c138e058fc3c0b624892882ab0 Improved title
712d72dc9aa8f2eaf116fba96014eab3dd788047 Confirm when moving
4ba36244435446894cde6c3e3199ca4fd7572ff8 Updated project and prettified modal
53e1c24233668a83f6e03ba8efbffb961ec2e20c Removed IsWellFormedUriString, lets be nice in the face of real user-problems (fixes #459)
986bdaa3213f8a7af6bdca93faaaf6fbc0466581 Cofirm unpublish (fixes #465)
10bcecb6caee9dd170072437dd7da87ef0d01622 Clearfix fix mvc templates
a789272b6f1612c5359d3e025ecba632df00a61e Allow expiry when managing parts (fixes #232)
5da13323a7683fc8b474f898eff590f451f9baf4 Filter parts when they have ZoneName = empty string
7ad40086592b189219006f9ac6abd8d34cdd4676 Tweaked nav css
e9c931e30bca3efdac29141e7bbae4a73f0bd297 Removed warning when rendering parts
87bde9cb4a9f69c25ac5ea31e756551b81045797 Probably fixed problem emptying trash on certain installations
063296ec6777b59818195f9e965a48ca94c379e0 Fixed some issues after merge
5b3503b42a9759114080ae5a78154feedb05baf3 Merge branch 'dinamico-responsive' of github.com:n2cms/n2cms into dinamico-responsive
06209634245e1b8f22816a39ede19badd2986bbe Merge branch 'master' into dinamico-responsive
c67f13501dcee8fa4b332acf36e094032f2c425f Removed commented code
762c8a2d230b7a1d323af45e9f13d295ee53ecc5 Merge branch 'master' of github.com:n2cms/n2cms
fe25ef5187d8f52338394842bcceb65a013dbca2 Fixed some unit tests
93396cad7f2ded794426b192636080e900285ebe Merge pull request #455 from petedavis/Drop_zones_with_floated_parts
fd1abd3b581003b542780b7ec722fef723a49d3a Resolves #463 by adding a null check prior to string comparison
58459c331cab72f6ff9009d87b65ecdf5e27c82c More GetRequestValueAccessor tests
63e3847b60d9dabed19af90b860adf3ded781545 Added some tests for GetRequestValueAccessor
c2d2c12812cdd77c4b991c369b6b0f5eab189744 Multitargeting .net4/.net45 for razor and mvc nuget packages (fixes #460)
1ed8e6887f76f296c632a5bf2298d4d635cd30f8 Fixed name refresh in navigation after renaming item (fixes #334)
fcd82b9dd7f3b07b4d1a9af4943a874e6862ba3c removed console log
651ccfbe8c45d04036dd83069deb9d1938486755 Merge branch 'master' of github.com:n2cms/n2cms
256d63cf43ec53730a9723633aa839ed085f75f8 Fix menu responsive mode
fbd90464e0b429c216c60140bbe49273aac2fbe6 Update pageQueryKey in the other place (re: #458)
fa498789a6a5e88c87399bcf9884a9161b1d17f2 Merge pull request #461 from DejanMilicic/master
55e9c2e2ae97cb7b0f3761c65e5143a67bb1b4db Changed default value for pageQueryKey, resolves #458
9f83fc123d951fed1c86ea55dc11aec43acfc568 Expand to renamed items (fixes #334)
42c19c5d2eaf09158a4df8824370952ea3a82b83 Expand to new items (fixes #309)
3e883b4fd16bd4a72bfa66279ea21b1d290cb023 Update several Nuget packages
fe6c257c398591e0749c65b4042185262790221a Update log4net
3edc4a93ee572bc2c7d740af5f3fd5a7fd906564 Merge pull request #456 from petedavis/Allowed_Zones_Registration
8af1e0aac4fe0af00e4264d57dd68aa441926b60 Merge branch 'master' of github.com:n2cms/n2cms
f17f9d4ca0d4234230b1b3b7349887fd822f1470 Remember info openness in profile
680f0bb63bf941695af566202da6d0229c80cde1 Change string conversion to Convert.ToString() (Resolves #457)
bf81a8c088a1e23f5f13c0489651d8c8be514825 Add allowed zones definition to content registration
c4f5178b0561d22296fad002086625e4b7e87f0d Merge branch 'master' into dinamico-responsive
19a7d42d9c65820e5f6590ba3160ddc0cba2d729 Whitespace only -- to reduce future merge conflicts
8bc123654422bb65bc21ecc3082a38fc5f3fb530 Merge commit '70f21c488f5692290216c1212e02903bd3c0460c'
cee73ffed361d4bd29ab606f0ff7cff6c1f05aed Allow separation between drop points when parts have float.
610925a3b49653b5b4c439994f5571161ba24953 Merge branch 'master' of https://github.com/n2cms/n2cms
fcdb4670bd24553b169e44bf1b9a15676881d4e9 Hide chevron when no child pages (fixes #435)
be2f8898dfa0c992797f415db8c451426c3cb5c2 Merge branch 'master' of github.com:n2cms/n2cms
ca835d35530afce731677106a9474bf4747efb2d Commented logging config
dadf60b4129210078978de2f1edc3752966dcdce Improved diagnostic screen
e2369fa9c92e6fd9dfe255c66d57a1aafda72ae4 Add seo fields to start page
cb14c25f2e45db792c63d5c12852e955344505a3 Merge pull request #436 from DejanMilicic/master
3ee5115adf058019328464c1e60aaba356693b45 More logging
17f27b4a74e70bad957fb5855054cac8ddbaad78 Merge branch 'origin/master'
4378834e83ea3aa1dfab8771a0b3577c45c68192 ICO added to the list of extensions
d118258ccbb561dbebadace4c108bc016f83a6af ICO added to the list of extensions
0d66bc87892069ebcaa8583e85c9297baca95ea0 Update readme.txt
70f21c488f5692290216c1212e02903bd3c0460c Clean up Empty.aspx and Default.aspx
60c7d4a3561cdd47d38a0650641b46a5e7bfa1bf Missing project update
720df0ad8f16c6cb59a1b50d20678237991e8c26 Fixed responsive header
2f55dbf9ae849886ea54e890b65b5fdfc3cfd6a9 Update url to reflect path
01ddaf2a1fcde15e6401244a20f745d1bfa0cdc1 Moved config from n2_host to web.config
f384cc91111f8ef3e866e557fb01def0366d783b Commented out cdn resources
0dc68399001cf535ec45fbc4696160bedd9b0727 Refactored resource config, updated angular to 1.2.8
6207be03afb257fdff8a3552986ae48b1ae4f5f2 Fixed links in info window
5a1571889b116dfdcc169512531a0dd33f91928a Merge pull request #432 from bherila/master
aa4d533c0d1643a6147ccacc3c83e322a3f852c5 Merge branch 'master' of https://github.com/bherila/n2cms
d9cc544072eeba76ae8cd5a97febdfd629c17e1e Moved some config to N2CMS.Mvc nuget package
e5a4f0bad6eb9171986746f9b69b095a40991574 Moved html encoding config from engine to host/htmlSanitize
6da3f0ee6d2a12d67bd0d6fd632e4f1c90010524 Fixed html encoded heading in list editor
0f7ad84ebc973e393ddee64b0ba0ac0580cb9593 Fixed html-encoded heading in webform templates
27dd58a2f875b165acb614ca1b94d9ae45c905c3 Changed default extension to empty
4496a2fabbfdccd2a0a9edaa9562799848a4e673 Merge branch 'master' of github.com:n2cms/n2cms
d2d7c2649deb5b8a6d419a0c65c5a26eff16883c Added more commands to search server
e8b6aa4b908a3b1380393b88f1a479e3907c4bfe Imcreased logging when indexing and searching
b3806f29e6ea58793447be0d2fc9f5f75ecd3120 Refreshes site tree upon saving any ContentItem (Fixes #334 although maybe there is a more efficient solution)
9ec00ef5c40b986d71fd0005fca1905695405e51 Saving permissions now returns to the page automatically (Resolves #345)
2f4a8c5b508d7db9dc332fd7d51430b81c867c4c Remove reference to dead code (Resolves #429)
e3acca7eb205c3e25e6e55722360e3f867104281 Merge branch 'master' of github.com:n2cms/n2cms
f6f341355b62b5f4c8e18567c9724234fe179b41 Temporarily disable safe URLs (Resolves #428)
c17dd4e9bf58744086708264069640b1a2d9d225 Merge branch 'master' of github.com:n2cms/n2cms
baa7a94ee17f9ccc55e9352ac6132edb530c8382 Razor install package adds bindringRedirects for MVC/WebPages to work out of the box with MVC5
f8f83be59aba7842aca2123c317e75b4ca433044 Updated dinamico and razor to mvc 5
2b594c430f84af183f67e7043055a5db0af7ba2c Format Edit.aspx.cs
4aeba8a7cb1691ee82e3d5e239a8ad585689cbbe Merge branch 'master' of github.com:n2cms/n2cms
46fd9c1f8c6635fa2e6ba86f1d3a8092145b296b Fixed some problems with wizards
0ba11ec4a68e40d56df90be999dc35395d0757cb Enable transactions and remove from parent templates fixes problem removing templates (fixes #427)
3333e130bf40cccd0c76ad153a26ee3637be6336 Removed 404 for empty TemplateKey since this cause 404 on a lot of pages
fb72b42ad0cf06302b7450fb5a3b2f9041fd3059 Fixed html-encoded icon when adding using 'AppendCreatorNode'
3122bf2d6a78b61e0b85671924be2d47d4bdf856 Merge branch 'master' of github.com:n2cms/n2cms
2abd65b38f99cc6a58261dd92eab5a1176c94d87 Fixed imagefloat again
61b651fda9d4b024a2972797bef8c414f8983068 JS cleanup
a22a797334de91edb668d349478be091dd641726 Resolves #417 by checking for null ContentItem before returning View.
390e4e4c6f39d8f2e2f4de672e9d499e13fe6ab1 Whitespace
603785a60aff9b23f4bfe2c53b262fe71dce9e72 Merge branch 'master' of https://github.com/DejanMilicic/n2cms
b427d17cce0d68fe4f6f09a289ff2e612f4e97a9 Fixed infinite redirect loop due to Redirect page
cc47ba8674b81de90a5cced8c037e04710762794 Fixed invalid templates causing initialization to throw
2b2778eebd3100a83c93d1b887087865ba8bc4f6 Fixed exception when not using roles (fixes #413)
33b665df22d8df318d76200e2ecc07304f6adc07 Fixed compile error in ImageFloat
4a2727506e4574fa577125e8d2c750d0721ed863 Removed code wart (fixes #421)
c7c9e2c2a83fe699bb05ccc2266e90f541a9147f Merge branch 'Upstream/master'
c3e80818101cc33dd61b4ab776f2bf1b7fc29c3a Merge branch 'master' of github.com:n2cms/n2cms
4fa8b277599fb774a95fdc9522d1f451731948bd Whitespace
b28eee424275fbf5870388bac83e221ed0da8ee1 Merge branch 'origin/master'
1ed3eeb6fa24b0216cb732488b2d3d6fdeee94e2 Closes #127
308231bd471468d33ec0404c3ebe87bbf373a94f Closes #127
129a708a4debcff0e9eee47a1693accaf893e387 Merge pull request #420 from jsassner/master
4fa08078e3b524e6f0789fd0147b26201a5684df Merge branch 'whitespace'
2061c1bed30c3266d802adee1ba8ea78ff6d2f5b Whitespace
3a32aae95a417bc8aa298cca75d8f56f7d85b8f3 Cleaned up resource file for AvailableZone control, and added resource support for the missing controls.
1a239879c4163aa7ad0b2ea4e7526b2596519fb8 Added tooltip texts
5892d1b396171553a65f27d036f1c71dc07594e1 Merge pull request #419 from petedavis/menu_enhancements
3581ba6f94e2070449a3ae7e9f310b7eff8591d9 Exception thown in the menu part in some cases when the current page is null.
e8dacb3d8375995e4b9e264f985d0870d0dfd28f Clean up DatePicker
080b4f6e6ee2f8202d960ff2e2b7fd971bbd9b02 Merge pull request #418 from DejanMilicic/master
b4abe31ab9722ec64647e609114bd0d43e1e074c Date picker is now using id instead of class name for initialization
5ac7b222bbca22ddb769080e6975ea491759a430 Merge branch 'Upstream/master'
798af31d7214abb6b9756d7790b4343f4fdcd93b Merge branch 'master' of github.com:n2cms/n2cms
aa938ae240bf686ccf4796a1836970db1d4332a1 Refactor code, clean up ContentItem
89419d100744f99df6365367147c3527689b25db Merge pull request #409 from n2cms/safe-url-handling
e9e91e72cbb0e77ebfad284da21ea6c65ac63930 (whitespace)
694974acf69d6f0e74d185ee85be126eac926825 (whitespace)
805708281c73c64d175104d4ef522ee4e0680707 Add LinkHref property to ImageFloat
5b5fd03c50cacb937cf564f4685aa044aabeb1e6 Update TwoColumns.cshtml
8f8e67ddb78736b3e4c269582cc8a1334da4cabb Ignore columns on mobile devices
0363d92cfca69825a04178b6dffe4f434041ce32 Added default classes for 3-column bootstrap3
b922a9ad91b5b223e9b6f4014ee114981c65809b Added default classes for 2-column bootstrap3
a1363eb004d49e9a22318b477749d79c1aa6a964 Merge pull request #416 from jsassner/master
a2f64cb8588d2c2a44ddebd23602df979510d4f9 More doc comments.
ccd4d01676eee6e00cfe6dfa6a0e990bdffb6080 More comments and consolidated some code in MenuPart.
a819b199aff8b465443bf6dc1c37b37f1dc6a1f7 Merge branch 'master' of https://github.com/n2cms/n2cms
15cdc46bd4c367d105bc5f876119b2725987e7c0 Removed incorrect 'abstract' qualifiers.
e9857e2add964cf64cdcc7c01d2a6637cd262d91 Merge pull request #415 from bussemac/master
1c043944cdae947cee652e452d241128ca802fcc Merge remote-tracking branch 'source/master'
591dd63995922954f62cbf871e347982d9b738f7 Another ISafeContentRender fix
1cb230b33fa347d58391c4b99ad85afd3121dc64 Correct spelling error and add comments to MenuPart.
db40e36ee006476b119cdf4029afdcc0afe98bcc Merge branch 'master' into whitespace
1e82e546b9b78d24e9aa2479e7fd1482df781cd8 Change ContentMembershipProvide to check the property RequiresUniqueEmail instead of the field during CreateUser
208459e91c9bdcb968fcc32100191446efd02cd4 Merge pull request #414 from petedavis/menu_enhancements
67fe519b7ea43f8a9bb3e7ff5c8b94995d339386 Cached menu does not render "active" li elements within during the cached period.
db94c124c5bdd27988c8e2a2cee7adbf4e676159 Merge branch 'master' into safe-url-handling
eca7684c9839eb368b16d43f5966334e25dbe725 Merge pull request #1 from n2cms/master
bebdee2488d8933abd371c78a281fc139b90d8ab Merge pull request #1 from n2cms/master
9f89d6483f38e9407a78fe01fed817caf9b6d27c Merge branch 'master' of https://github.com/jsassner/n2cms
e9b3dae6fcf0e8222cccbc8b64edc80276fcf688 Use GetSafeHtml instead of HtmlEncode from ISafeContentRenderer
e25a81e1c8ff2c74ad29ebe89b3df6af955c792d Added caching to MenuPartRenderer (#407)
b78b7ca7ede3de8d885092eb8b644f36ba9be874 Merge branch 'master' of https://github.com/n2cms/n2cms
d56c954d8d866fb4ded0b6a3a5fcbd5b0ee370da MenuPart.cs whitespace
ba77ce5073ff7e59a0304d2e0cdf6fcf6503c0f5 Rename HasChildren -> HasVisibleChildren (new name is more descriptive)
c468dc21dfa48cdf48019c63a8545e7958dc06cb Merge pull request #407 from petedavis/menu_enhancements
048cebc5cf99a476505a6b3ccce42cd8592743cb Enable showing carret on sibling pages if they have children
f583fb684934cd14988721cee36312987d127969 Toggle target based on ID to not conflict with a header nav
d6b432f51b2b3c7adc40a53d4abdab7ab663bc51 Merge pull request #406 from aweber1/master
0270eeabaf94d31a028112f6497a7940fd309fd4 Set 'GetDescendents' method to virtual to allow for overriding in inherited classes.
f914f467632a90299270f163fef559ef8256fe0e Moved deprecated styles into bak folders
0a6776e1c41345c5689d029cf12b966f3fd26b94 Fixed url rebasing in css compiler
93c1eb83fa0928cd8afb5d1269d6fe40932100f2 Updated dinamico login form
c8605a1ff0eb7e004f659e93775330a555a0a856 dinamico CSS tweaks
5c5d2c1eb7ca682fc8cfcb2652cee17fc17c8c2b Updated to boostrap 3.0.3 and reorganized/fixed things
60bc5af83e74df00b13c2d371d2d7d8af5e9ff40 Fixed CssCompilerHandler not handling single-quotes in urls
ca46856cc8e0dba86e9f1fbf845b8b132435e225 Fixed slider on dinamico responsive
b73504b65dc4d19361bbb3d0434053d945e7ed74 Merge pull request #404 from jsassner/master
68c34958d5ff68cfb44b15bc38620f54e291efcc Add ISafeContentRender encoding to Trash can display.
8e54cc015a57b5152c119b6f85662e838bfad3ff Merge branch 'master' into dinamico-responsive
a1984ccc430f331c73b621a5cd740a43fc9e5f54 Ensure newline at end of files
4c8b82566b6bd214b2bd78994c2321be8b3c3086 Fixed line endings and encodings in aspx files
f73911a386b6337dc4cffded6c3376bbd4634a6e Improved html parser
4479ef42f154ad8a2a239e12190efd4e66c2a800 Fixed some unit tests
e99189891bf2382c2a79cc1bcc3c2f6efe51e2c2 Missing sqlite files
1f1dd79027ed43e17b3e192892fff9676dc95bbb Merge branch 'master' of github.com:n2cms/n2cms
008fbd36b3a9796948e8c0a266c3bf91918b890b Fixed tree refresh when creating folders and uploading files (fixes #393)
7b445a62a012ea5811e5af09ceb00f661ddbfd60 fixnewlines.sh now writes BOM to files
4657b3a89cad83b3930da401f5eb75c1b115496e fixnewlines.sh no longer attempts to convert binary files
98f8a2d489d314c90602c5d883f383c92fb566da Convert XmlReaderTests.cs to UTF-8 to avoid binary encoding
193ea90617d2039fa5b8b30714608f9f12087f9a More whitespace
14ce81f943cd19731a001498e90c3115284d068a Merge branch 'master' of https://github.com/n2cms/n2cms
008b68873040a09c7a237116fa54b67d17d958eb Merge pull request #398 from jsassner/master
bfdb3612004fd45702dd4485fc17d79f6811557e Merge pull request #396 from petedavis/image_upload_registration
d3d9acec44f87230ea2a46afbc090d987ddbc9e1 (whitespace)
7aa2f1ebf923ef0a8072adfc4e68c3cbaf6dd5e5 Merge MenuPart.cs and WithEditableTitleAttribute.cs
b5c6612a5c3a5aa8b73aefd27b8119e31a35a88a Merge branch 'master' of https://github.com/n2cms/n2cms
0842025d296354cdd0c8c3bd70fa4aacc44b1898 Something went wrong with merging
82549a28fc41a3c3e5343fdf47032474e8876ce6 Merge pull request #378 from jsassner/master
44053b0c60e575fddf0585efb9330892e1eb550d Merge pull request #388 from petedavis/menu_enhancements
948d57e2f6ded26d36aa428819af65e9c1f20f45 Add the ability to define content with the ImageUpload editor.
70d01585f1521a6de80e2c87592a72b3551afb9b Fixes #387 (Long directory names not visible in upload folder view)
c35d4b5a09bf271a65398b59e6357504d08f8639 Merged
7735cdac29f210774b3d2df61d2aca79ece0e814 Added scripts to fix code formatting
40cdc460689f580df601e10377c6e14a7993231a Fixed one issue and verified build is working
ce881a37ad73cb648eefe690a81a9b6df8b0aa19 Merge bherila into master
e37573de90bb70198aa5e488e100c1e13a308065 Fixed tabs and newlines
982fe1a7b0ffe8cec648e162ceef05151cbbbf62 Merge remote-tracking branch 'origin/value_type_editor_support' into menu_enhancements
4c402cebcf0538e7bca2cf7b6d79105970ba53bb Merge branch 'master' into menu_enhancements
507317aabf2f4cfe2a83a6313a8dce503a87d5a0 Fixed issue using creating itembridge with windsor
f8bd1755596c7aac9ee7d4b41cac759df377cd88 Fixed issue using creating itembridge with windsor
3657d50e56036673a52682e28fe3f9e71328681c (normalize whitespace)
e44cfa7c03142e84b75bf5d08eafaa2010aa0243 Add responsive menu that collapses under a toggle button using bootstrap.
228bb5f3b0e26c955a93af8a047179ba256cad05 Fix directly nested ul elements when not showing the root node.
af1c2da003f6cc0521bd3edc3a86fcb8766fa5a1 Allow changing the MenuPart name as it is used to control the HTML element id.
ba488888fd112c856efc53ed2d925e8927ee4cb5 Merge branch 'master' of https://github.com/n2cms/n2cms
a3c8ccf6de03b6bf1631f3aa1e7e8afb95dd634d Fixed NUnit so that it uses Nuget properly
24515c680c2b60b891a65e1e77cd491b73d8cdca Fix whitespace in MenuPart.cs
8c433fd99a01b934adabab9b5a248c45515b4338 Merge pull request #383 from petedavis/menu_enhancements
d6468e574335ca1f5c435b4fda14effab4ee274c Allow for css classes for menu ancestor trail
63383938c0dfbdea4fd781b5f93cab3a65679e1b Add ability to nest ULs inside LI's, and add active style to ancestor path
d9e5a298955a814270f7edde64be694fd4eb59aa Update LoginUrl so it works when N2 is not installed into the virtual root
b237a49251cc8d1912da8c42a0daee3b48e22289 Typo in comment
25623b8bbffd5e0223f43352c0406b29ee9beaea Enable use of value types as for models.
8d8298e2fa85ea156dc87955274a19b179c1883a Merge remote-tracking branch 'source/master'
c1a2a0222338d16e8cfb501af85aa3fed2888520 Forgot a file. Remove AllowHtml usage
d3192da2f63ccd63230f865ed4435787b9a9b1f8 Unversionable parts are deleted without creating version of parent page (fixes #379)
70c1c79829c6da361f38417e26e1c84692b65630 Added guard against strange null key when populating proile (fixes #380)
fbf7449afffa370fe2122e8329268d28c1b12e27 Removed duplicate <validation/> tag in config transform
8481b4ea45ed5595e23ac0dc8b9793300b227a39 Set forms auth when using N2 to sign in
7d7ea8cdfbdfa2f2e1c8de0bdc1902a5310f9703 Enabled restore packages for MongoDB.csproj
6a42259a3fbcbb5c52695b295c78d8504d03c578 Added SafeContentRender class that is the base for render safe content for the client side. Renders Titles, Meta tags, Tree nodes and Edit mode rendering using this class now.
b51d9fbed704be66c3e9fe05252cf34fa9e4c090 Added value type support to tiny ioc (fixes problem creating itembridge)
755181eadb8dbaba62a17a5da5980055b2ce2221 Made some changes that could possibly (fixes #369)
79270cf50449e93d98730b1c2923ab0488844b3e Merge pull request #376 from jsassner/master
eef1b71b88482ef240ca9b18c2f23b08580c96ff Remove binary packages again
73bc2a4faea60f86173fa1ea0f87cdbf6b51b243 Merge branch 'master' of https://github.com/n2cms/n2cms
c3cd9e4d0d753d85fca396912c35b2d2c72016cc HtmlEncode design mode header text
bf61cc4aeadca687fba30e7df8e2f1fb92b3eb90 Updated .gitignore to ignore nuget package binaries
006872a80205094a841be2559b5bf944946d043a Remove binary packages from repo (not needed because these are now auto-installed by nuget upon 1st build)
6fb324528da59ad4cba5f6992f80fad898d07676 Enable Nuget package restore on building the solution
455b9685935ebffe2d320e2215b39c9df2851cbb Add ability to display custom text on the login part when the user is not logged in.
52d5504e05b327f47c6f7cfbc84b04dde0ee506b Adds logger to ItemBridge
f2ac76be0d855ae6dd962179a8d91b1a4aa67eed Removes call to singleton engine in attribute constructor (resolves #373)
ae2c13cfd96e4acca2b4805539f21b6c8d5a4ba5 Merge pull request #372 from petedavis/admin_email_error
161976b6aa44641a065990b73a6af1ddc55ef046 Fix exception when finding user by email and user does not have one set.
51cafa2b1db318620457671efba9cdb2b240fad5 Merge branch 'master' of github.com:n2cms/n2cms
8411eb94617d4c1d5137e690f342774e33bd5c73 Adding missing packages to git (fixes #370)
6feef2df1812bf6621cd7e9b38d4e5b8707a29b1 Merge branch 'master' of https://github.com/n2cms/n2cms
e8ebe00d055b30d6be2590ccdfcfb3bf97f5fa74 Added configuration option to support HTML escaping by deafult.
a9794f7d9ac87aee8923990f675ca9d804d12e66 Merge pull request #364 from petedavis/Issue_363
92c23cf3f054e7708346f2af1e6e1896a119345b Merge branch 'master' of github.com:n2cms/n2cms
cde663307457797b0ca6a0b9dff4b532aae0843c Fixed problem publishing (fixes #366)
44d4fd62a761b91e195b0f05ccb211ac355f57f0 Fixes #363 Invalid cast exception
afcd659a7fe37c0f08210d42535e2fa67e15376d Added AllowHtml to EditableTitleAttribute (resolves #362)
0dfb57e3c64e893bd7ed70d1c3e02191db1bcd44 Add default admin user to Administrators role upon setup (fixes #356)
a9b51001b32435137eeddf8a251a3382c8b8632a Support HTML element IDs on slideshow images (Resolves #358)
54dc29c4dee5df41cc96423183a20c27377957ea Remove unused parameters from EditableBuilder (resolves #359)
fbce133aa9b89a563eed816bb67c35ad36c57b19 Merge pull request #350 from EzyWebwerkstaden/master
2abc2892bff19cef46ee6146dd1ea46a29b50bfc Assembly version 2.5.10
4dea972fea1226757f9bd995cc6b1878f567d485 Added roles menu localization
faee00626738b811ee8bfcc343e0d5f870841bad Relocated change password code, created user menu and added translations
2c1d89292a5fad153af9aa7060f009f1b47d1dd9 Merge commit 'd68d2289ab407793032c3373284025b82b643ea7'
7020ccd4660b8de16b01268eae68eec7001935d5 Merge branch 'master' of github.com:n2cms/n2cms
83e8a86d58aef8cae52cafde8b08cb194af78138 Merge branch 'role-editor' of git://github.com/lundbeck/n2cms into dinamico-responsive
76b05d213740930ff74194ad5640b906f7d43530 Merge pull request #355 from lundbeck/editpassword
3173fe323db1c4bc9e7a562dbb8479c894db4484 Merge pull request #357 from petedavis/extend_cdn_support
35015259b5d5cac476bf2308d9946ccb040ac9e1 Revert angular to 1.1.5
4c63ea922fa25c5a5c5f68bbde83619e2f883465 Add suport to use CDN for angular and bootstrap
db9d5f5008655dc0b5e51767bab89ff68b82208c Enable any N2 user to change his password
d68d2289ab407793032c3373284025b82b643ea7 Adds editor interface for membership roles within N2 console.
af8fff35ac878a89835a400e87582ca8f5f283ba Fixed definition sorting in the event of broken IComparable.
953feee9f6feb5bb721767f335829b5d9b76dfa1 Merge branch 'master' of https://github.com/n2cms/n2cms
cd8aa6b173847a7911e8dd1f9d4d1040c972dbc2 Added ID property to SlideShowImage
91a73998abffb49a11b785de718e27b739d99775 Changed to IIS express for dinamico
047403d4a2a4f842aab1e8d3be1b9a6d85405943 Merge branch 'master' into dinamico-responsive
d284556758d20628c5553df6015b6b68e8dbe60f Push config
ce3bd26517c1bf7e6a01cfc74fa421f9bcb500f3 Insert ValidationSettings:UnobtrusiveValidationMode during installation
778a525f3625d4d2f81630f5196697a31237f329 Assembly version 2.5.9
caa389e4224ee83b000d32ad509a3da35464c8e0 Selection memory
333cdc9eb077a66e3a0cd4a5bfd0dbae788f4c24 Scope feature
8c9b3f1637956f47aeec12aad43f11ca461f6604 Fixed login with content membersihp provider
878eaff561b4c2548875e7072a70f310f6fdb2a9 Profile flag on user
6a47efaa3027f60eaa849488e65e70a41ecca8f5 Some tweaks to user handling
31e9bfa96b1cddf8521db198f36e3a1759c5c4fd Removed some comments
5c8c04b4a00cae14d737fc2c9bd57c834ef2923c Remember preferred edit action
e60e19e32a697476b6dac0026fd5db6dc5eb712f Client accessible personal profile data
9ac7596fed9d6dec00c9f5698856cbb3eb78b69c Fixes #331 per issue at http://n2cms.codeplex.com/discussions/464970
8974bf5f32ebfacf9f0737d3ccc2a7a3b507bcca Removed some obsolte classes
32887d99396caec0f2e24aa1374b10cad3975dfc Removed some unused scripts from dinamico project
52cff9d0030c01afb31d44df04d666e47390665b Fixed some unit tests
0531cbb5010e0310eae4faf6e927155dbb8ac5bf Assembly version 2.5.8
5a4284d8fb4497bdbdb34e5da2a38bab907bf11b Cleanre refresh when (un)publishing
e19841e3a48279af3e8cff22add06a9db2c2bb46 Make edit menu remember last used sub-option
272d7d67fa6d189a4d3d98febf49ac2ad6dc1cc3 Fixes the fix (fixes #337)
0c960b9dfccf5733a435c12564330a0dcb3517d5 Merge branch 'master' of github.com:n2cms/n2cms
eb9b722be5e9c7a12db73668d7734573f1eedfa6 New api commands: branch, tree, ancestors, parent
51ed420d6dc203cb3245dd2578d8fbe4b51fa0d0 Fixed line overlap in error message (fixes #333)
d32c98dae6ec20e325d8f6a8d4b1dc3b71aa8dba Enable retrieving branch through api
3dcdd126937ae13622f598f77cdc0d23bad4cb45 Merge branch 'master' of https://github.com/n2cms/n2cms
528eded9e72aa134a73fdb93f76414e8a7f1cc87 Fixes #337
c4b3ad54b83a00d4730cedfbc4b6eea42f12747e Fixed css syntax error (fixes #335)
f19f0652cc4795e6d27e68f50278b5b8ad339af6 Merge branch 'master' of github.com:n2cms/n2cms
56e36630633ed06e55e508488c739e14909b952c Fixed some problems with users
56aad1c295102ad5adc89c543ec14c955e539faa Fixed problem with gac-assembly beeing picked over package assembly
97f7a616a475eb98c8da0c6cdd0c0d66c9834fc9 Merge branch 'master' of https://github.com/n2cms/n2cms
89bd0bf99c638006402c254b5182a79e7b2567aa Proposed fix to web.config transform
5bf266557d163eacfd6a4bae60021fb9fa1f0254 Merge pull request #332 from abadiwidodo/patch-1
460d00719f9d219134acd66e4a59c3c2617c75eb Fix GetUserNameByEmail method for issue #331
b68770bf3903f3c7946a419fa1a973adba01f917 Merge branch 'master' of https://github.com/n2cms/n2cms
326031d2e787f97a06cb15de3a82462d436b11d1 Added more checks to DroppableZoneHelper, fixes a NullReferenceException that can sometimes happen if the browser gets into a weird state.
044a6ecfcd06f697f706b840437bfdf503e46650 Removed packages folder from root
2da005848d1b10d9fd0f5bf2160485953b0cdf9c Removed N2.Everything and re-linked package references
f9af8ef68a1a4aa795e415167d42b0e67c0b42f9 Fixed frame in frame when deleting files/folders
5ddd677ec69ab949ada970a00a8264c11f2eaccc Removed obsolete files
09ebc8ad223bd7504294676797373938bbbb0dc1 Merge remote-tracking branch 'upstream/master'
664371a7802da328de77cedfa56393cd2e688ad3 Merge branch 'master' of https://github.com/DejanMilicic/n2cms
4a66ddeac7087978903e6a8d5bc3263ee212349f (Whitespace)
72b4dea3fe7757de78a2a1099908b28c4604b766 Resolves #136, corrects type check
892e01090a72429c33088b05a640fab0b89aa55e Merge branch 'master' of https://github.com/n2cms/n2cms
51d551caa6d987729a2a84c835f5d696346b83db Resolves #323, makes Organize Parts the default toolbar action
b9fa3a5864a003f8d7d42903406cb78313a494f6 Throw -> Delete
1118b0cc57f1d48aa296d077233b32f35178e143 Reusable way to apply selection
5f8d4020d79c030b6e39850526bed52abaeb21aa Ensure valid selection when posting (existing selected=...)
3945e161fd0b86c15d70d588c92ae7ae11b0c876 Changed default permission denied http code to 404
f3e7a163cb611f47232032c0183a3f332bab89fc Merge branch 'master' of github.com:n2cms/n2cms
f7bfa64017556ec5b5db851294457f09d1d0a05b Fixes #324
5e65c42fd8fb86b29c375ed155f2a4743d8ddd0f Fixes #319
4f3e3351743a94042668a6fac9c983b14dde8202 Caught some edge-cases with refresh logic
9d47633e5c4c38d54a7b482ebe78c62ce3056860 (Whitespace only)
01a77d4468adcf1ddaeb3dbf3753170e1e0e9836 Merge branch 'master' of https://github.com/n2cms/n2cms
4c9754eceaa4655855169299a16008c14f94fb68 Make the cast exception in GetDetail more helpful for troubleshooting.
2c062b8259406a019aaff94b579b932546fc12a7 Include lucene search in build output
19adef3a03e5823bba3385e0c1c6697629d561c4 Rudimentary POST/PUT support for content items
60af384f7fd342a55b35e477d29842bfc987137c Rename refactoring
dc3dd936f877785e70f0944b8bdc3737d81143f3 API GET /N2/Api/Content.ashx/3
497c22316ea35747e4814a462a410a7d213906b3 Fixed some slug tests
20a7cfa366585129fd95eebcf72f6eb547c9e025 Fixing problem with sqlite interop running tests
9826f15b5268228aa774d76177417d7f668ae450 Made version list show a published date that users might expect
4623c74b9e0343e514866bacc68a9bdab79c22e1 Assembly version 2.5.7
c0ce397030639a2fedd541b45652d6cdf4dbcad2 Improved backward compatibility when using [EditableFreeTextArea] toolbars
bb872600e50a4222eff69de45c51cbb0e48bbc6b Keep options from option providers in the new UI
22de10c0b2fb07c40d5450d4238fdd1bf2285163 Removed debug flags
fa8f26a4e70e692347d452ae4ceede50ca687456 Updated upgrade instructions
122fa0e3c3abb4ca0765b17289d44cc74dedc354 Made lucene search separate package
cbadd23ce1abc3178597870f1c7115e96257acc9 Updated N2.config nuget package version to 2.5
a32f7c7ae2cc15297a2f651182f8125ef2d16405 Work in progress
176331f73eeb848a0043bfaf6be2ccd28576fbed Updated xsd with remote search config option
dda2f2c3bec62acc4b58da3141d36b4f1910fa32 Changed name replacement away from aring -> ae
3523a095bee9fe8eb089dda54db59c502c6c5a5e Fixed castle reference versions
a524c9407f44d9ed056eb4e05437c86a0abf8cf4 Merge branch 'master' of github.com:n2cms/n2cms
5e88a9227717a20af9950b58359ab29397e05750 Fixed issue with forms in webforms project
fec53ed0da1b8bc2f5d79f17cb28deaae36c4ab9 Fixed issue with forms in webforms project
3956c796f580b232034a49391a4352debe1a33e9 Merge branch 'master' of https://github.com/n2cms/n2cms
09bdf7385cb57411ea7715c1109a7beb38bb8892 Improve compatibility of MenuPart with Bootstrap 3.0
44a331204db0d39c0079140a5f68531d715a5591 Jquery 2.0.3 -> 1.9.1
318c85a48a3c42096c195a2ecfef78022191e9ee Merge pull request #316 from abadiwidodo/patch-1
aafbe5a58c524ca1cc100029596f7f8c2de8dafd Update Nuget packages
b5edee388dd7536cd1b372f5adbef79fbf48123b Bootstrap normal and responsive CSS paths mixed up
39c7901319ad46e540337780e83f76669d49d9f4 Merge branch 'master' of https://github.com/n2cms/n2cms
2cd8c5fa3423ad260ef2b75013c829327d7e3d9e Implement RequiresUniqueEmail validation in CreateUser()
603b2e94014a3243ee66b8796e291a6c34900076 Renamed some image files
6d3ec46d19f14850cc4e43e9ddd1f58516ef20cd Resized images to match new design
485a2e0d5d8b80e82c4e891433bbcee01db730a6 Fixed some issues with rebuild image size migration
13ceddca61a1c5626f6c01b75576297bcb1d8420 Redirect rather than display language on start page
dd735e1ad1ad5b00fe3a6d7fd10000a2c098d3e6 Merge branch 'master' into dinamico-responsive
68ac821477f990f6241614d212d425f2cecfa638 Merge branch 'master' of github.com:n2cms/n2cms
4f489b82d767f6b1891c18863320cf5667355966 Added custom IViewEngine which should allow usage of MvcMailer (fixes #315)
bd48b0fbfaa51d7b803282523bcbdeedb4333cd0 Merge pull request #312 from andy4711/master
e07a4fe31e6e6ed75da5c6f99d34a78a0ab85468 Typo
7988fde927c501485cfb78065bb2c3e524198d82 Merge branch 'master' of github.com:n2cms/n2cms
426582b79baf2f32d612223a3df12a1181f85c44 Merge pull request #302 from andy4711/master
9d6ee81e430879e6abdd7e297597ccc089d552e9 Show button for some missing interfaces (fixes #308)
233d154896ac3f105da0e2ffa5e180688dcf7042 Tried to fix some upgrade issues related to authentication and credentials config
9a3fba0a4ff98378613b1bf3eff6a288d28fd555 Removed static members from free text area control
fcb16eb08c5483dd7c695c2d524aecc63e48270d xsd fix
f2261ca080ef694ab638a1f48d900ae8d7865e34 Assembly version 2.5.6.2
2ac73b4afac334e1d8cfc754e2a800f1d3f64dbd Allow custom ckeditor settings
84c89c030813695b3be2576ecf188eccc2c830be Allow configuring 'allowContent' which prevents striping of some ckeditor content
5587b736232f7ffac411f59e1dd33056c96ccc69 Updated Nuget packages
924b8e7e1e02b5164e7488357799988d34486d17 Fix the build
d221ef268a499f71e768af4c0cc8e494b784b055 Merge branch 'master' of https://github.com/n2cms/n2cms
209de0e28b5fb50b7195b23cd953621c386db2a9 Exclude local test publish profile
171f0a1e18aeed4e2e2636f7ac45df84a6a22fa9 Include ckeditor plugin folder
df31aa107a48d245c861504b244019235fd21e05 Allow managing root page (fixes #301)
c2a9f6130043026fb46725f8f42bb63a56a07c38 Fixed missing action buttons when updating references (fixes #298)
14d4f3fc6959591c5d231530b51e2c52d3cd48ed User node now links to user list in management UI (fixes #299)
1565acbdc2a1a467f1ef556b836b1cc1bd22c2e6 Show invisible pages when previewing affected items (fixes #300)
a29dbb28e93f79117f8239527cdee5da3ad4ca3b Upated jquery to 1.10.2 (fixes #296)
f3d0de3fe608f5e39047b46e2cac37b049529851 Assembly version 2.6-alpha
593deff9edb1db60e344c831f7a2702fff1698c9 Merge pull request #295 from andy4711/master
cf683fac2702bbe21cc6ba4b0250c534cf7fbea3 ckEditor not working with nuget package - fix
18a9a076c79bf057f5cc59689453f0a1660aea72 Merge branch 'master' of https://github.com/n2cms/n2cms
8997dd707f06985e029d778711e414eb27eea69c (whitespace)
e8b2a618149588fcaf65f36868c671f274696bbb Added mediaelement.js per #292
9558a322b96777df3de9dcf76b8a2d7874fd12a1 Make FreeForm Twitter-bootstrap compatible.
db8b212e3b1f22a48c0fe45d9539d35401b04290 Merge branch 'master' into dinamico-responsive
e113a52dd13b34e6a6bec084c7d814e4c6b57504 Fixed some nuget installation issues
fe71b36c1d9c08fa8c811d6597f0b5675c3a3925 Merge branch 'master' of github.com:n2cms/n2cms
6041ae32c7922095735a392d7522d16bfbe75629 Added tokens to N2 client API
9d6aaa53d50080512775cb74fe2fc5c8ba7f1a84 Assembly version 2.5.6.1
1d0221161e96e8e2b1ea7a25dbef1971eacd6c69 Updated nuget dependency versions
908139a73a3b2bebe04be893fb81d764e4f470bf Add newline at end of batch files.
1a89625d3a07a1e9ccd0953b878c143faa7ccf49 Fix issue with XDT that resulted in extra httpRuntime element.
32f482db099d872d68cc3db6bf3498850448016f icon-anchor => n2-icon-anchor
fe2922966336b8726ca1dc4776215c987c4794e2 Merge branch 'master' of https://github.com/n2cms/n2cms
ff94be4882b898c76d67d781cd17e94cca2548af New HtmlAnchor part in N2.ReusableParts
53e52b7264f6d7c069c1628fc5351887c2232f9d Added a batch file to compile just n2cms.config
7f9fc4d79eb35a7af9214efc808f90993a95b6fc Fix a few issues with the XDTs failing to upgrade.
224cbb3b1ef409cb26b0e4744a2ce217fb7d2570 Made VariableSubstitutor not case-sensitive
0d4fbe1e387a1a8f16ae14c23b881cbebc21f817 Merge branch 'master' of github.com:n2cms/n2cms
9c9867841f96bef284bd4efc54481075959f98fb Added bundle package bundling, Dinamico, Management and SQLite
4307ddec4d5903d114dc89053a7a39b4c45df8b6 Fixed issue redirecting to draft when inserting parts
df83c9523887e4b6a374379d4af00043a3357968 Merge branch 'master' of https://github.com/n2cms/n2cms
65e1756b1e53978e96d87f252b11ded457d72df5 Make news container group by months.
f2ec9fdcb788189bd2de96c9ea37211c4925ebcc Made some changes to smooth upgrade path
cebc5d770edc5870ba0d5679c2271d56fa46df44 Merge branch 'master' of github.com:n2cms/n2cms
eb7a33a24b70e3450cb7925d8c2ab976694ff9e3 Fixed some nuget upgrade issues
dca76331448f4f041ca562370ae9cc6a980b5c72 Fix: xdt uninstall loses too much information
d0b290f690d73d9806def084dda47e9c527f96ae Remove dependency on N2CMS default package from Dinamico, go to v2.5.5
992129f706ed9fc3add405a22104185e46beb441 Got XDTs working. Web.config updating is really nice now :-)
d2e2e5a60a5e9f6cb6c0c7b908f8499ff7592619 Initial XDTs
684aa0004a80f871c2372c628044ec4955038dac Moving to xdt web.config installation files.
48d70355faece677d861f4d8330baf88d2935d65 (whitespace)
0574bbb0781c39c72b5993cc94f3af15921a80e7 Add required configuration element (fixes #279)
d9303f35f1db56cae41e2fd10c28954b3032dbc7 Create admin user via web installation rather than nuget config tranform
97d1f189ada271be740778d87dd4433adb60413f Config transformation tweaks
3b4fae18295cf904725da5d8881c38612e8ceb1b Updated nhibernate, sqlite, mongodb and castle windsor to latest versions
fe0358544c58620b5d4f13c03b4240de8c01fe43 Merge branch 'master' of github.com:n2cms/n2cms
df99d8d0ac59fec08e84507f53d01b438a3fa17b Merge pull request #284 from andy4711/master
18c8c83df14bcb32a3bfcee238d1176bf6959aa6 Added option to create per-item wrappers in zones, image css class on displayable
ea1cfb18079e0264800f61e73633bcaff713bf74 ckeditor config typo
11af7130e9b35c425b904689bb1fd103fa3e0558 Merge branch 'master' of github.com:n2cms/n2cms
ac025ce86bef6c0809d350cf976766ceb10da373 bootstrap3 in dinamico default theme
5700a3feaeadff9a5568533af84eee7176bcb299 I've programed some configuration options for ckeditor. My intention was to give the advanced users flexible configuration options and user who don't want to deal with custom config.js files some simple options.
6a5e47cc4fe5fa7bc32cbf38c6ef2f5ac0abdd24 Merge branch 'master' of github.com:n2cms/n2cms
d0425d128e2cf5072dfc6c587882be51ffaa65b3 Add infrastructure for ckeditor config.
0ffa6111acec76057aa45e9fcc2e08d71c73ce01 Merge branch 'master' into dinamico-responsive
8fa47bce622d69c119f76d872e002fb4a6997a9b Fixed issue where base definition could be wiped by a view editable registration
5b4e3d7295484f27d06d958b1ffc4d4865bdbc50 Merge branch 'master' into dinamico-responsive
db800d10251529bf83effdb1c02dc5e947c3ec28 Added migration for synchronizing images sizes
61692836cbd27c97c96f8231bb2c6089faa411c7 Added option to create per-item wrappers in zones, image css class on displayable
f276fbbed79acdffa5b17ffa15bcec3a7e0af5cb Merge pull request #280 from bherila/master
391569c46e1421a29f68c1c27c585ee43b063831 Fix the alignment of checkboxes on user role page per issue #279
f6fafaf3add080f121334ceaddf8ee571b74ab6f Increment assembly version (2.5.5)
4672cf8165c80e0cae4c748996d4bba562f67fa3 (whitespace)
8d76ff35dbcbb8c96e0d5358904cedf35c354cf5 Move docviewer out of <host> config section and make it possible to have different doc viewers per file extension.
d743c38e8d1c9171a04f8999248019947caf8356 Tree refactorings
30b5d0ca1720d52dfe56737eeff30f30112f65c7 IPlaceholder interface
e42cd4de9ce8fdd623c721969fbc5dc657ae4f7c Fixed issue with duplicate jquery incldes in some situations
a7a18a20075fb0add211e7d16321bd9d75ddccd7 Merge branch 'master' of github.com:n2cms/n2cms
c518defa3878d7f331d6c210a1c67d1002665d68 Fixed Secondary toolbar hidden in admin view (fixes workitem/34349)
36b063f307a241b09c923e9bf5d6d793563413aa Merge pull request #275 from brianmatic/master
6fe12f7b0300926496d70604aecd143f4679ea3e Made control panel more resilient to border-box layouts
dd6d72a57273ba09370aa33d93ee765e0fb3cb18 Update README.md
f7ccc73be24bb74f013fe76a8e23caa81e97f2d3 Update README.md
af4d24332467ee110bc42dbbcf65685797ae5b20 added content about naming project that use N2CMS
953731bacb89c5116f7df32cadae1b073661921f Merge branch 'master' of github.com:n2cms/n2cms
162ac1dc0b383bd6e15d64c6e9b21fd4c6a786dd Merge pull request #274 from brianmatic/master
76f085fd67b8a4e948a5b6e1e36cfd1fb2e04853 Update README.md
37b4003c8a95baed24be82a87d534697fafcdeee Started FAQ section
b02e217ec3a5c6b968db49a2c7da8451a7ee0d68 Hide language when no language available
ab8921eb0ec9509df5929a2eacc9551bc329b2db Polished action bar
e0dc16753c910c00893020c9738674163fc8426c Updated some icons
dd3f303ea06adc1f77e7cc4cab1acacfe5604ae7 Merge pull request #273 from brianmatic/master
251f2acc265d833af23cbe1ecaccd332f238e865 Update README.md
c73f14d70bc88602ee6f854dc097563e96923678 Update README.md
39e0464b1b19e3939804e8e84bc4d5d202ef2b61 added link to more api examples
4ccc37686412f46d91db3f030fff61679d4cec21 found a typo in sample code
e3e465fb4e1567f3f2dbfe7455efcf30a0120344 Moved _ViewStart to themes folder to simplify custom views in other themes
b8d6dad9a92917edb71c43caf820b8449d1e3f93 Changed license to LGPL2
6d200c35fb0afdfd70adbfe248d661ffc1c2874f Merge remote-tracking branch 'brianmatic/master' into merge-updated-readme
ecf279d53aa8138222dd3a800ad465fc4ea2cfb4 Fixed so part adapter for rendering is resolved per-part rather than container (fixes #272)
8ad8a8acc799460809c5c070b11ca84f9e7dd72c Added some type forwarding for better backward compatibility
321aaa5c1b367add0a2085619e2723da3cca096f Prettified info box
761a7c36eac3ef4dac45bd35078cdb8d56832808 Clear expiry date when publishing (fixes #253)
6076bfcfc6f68998930d0438cb2b19387af1718b Rename README to README.md
f10a40da40b7a40f9f2d07424d87fc7be195c3a3 Rename readme.md to README
987d0fed1e559e52ce206f8d5e7af275ebc4da65 ignoring .DS_Store files
d7139eec39eefed572d483a97aafc3c7d46f8fdb changed readme name
1ae4c24bf9296b3514cac2a9aca21a7ea2d91fe1 changed name of license file
d3cf0fad5fb12f11187bfed771c604379090636d add a license file
4cf31552afc90fc059925b13ee8a30dca0bb2e10 tweaked content again
2b1a756cd28ac895ead7f22a2e26eb707de4de45 a few more edits to readme to help people understand the value
1972693b1a4b53763aecc776794979668dc69406 more layout changes to code samples within WebForms
b6d4aa50574b48a05fe9cb135b2b7736442ea51f making examples as accurate as I can
f5f9771c1e3637a31969910b5cb328df4346967e updated webform page example
a542083cb6a62e98fc2e5e7d1bfda45055bd856e found a type in readme
aa27442f7ecbf2953f84dce8c36e1a4cf8308bef added api section and move user experience to the top
bfb5f2b3e3831b794f060ce5cf6f20ea92ba8206 Finished final draft of n2cms readme
c5d73eab3c2ff2d44c19b9ebac584a7f73be01b8 working on updating content of readme
119d3b46f8e4dea712cde88edad938f725bd38f2 working on code examples
8b3634cf50ac5e06dad836829e2be9c2c9fabebb Update readme.md
8b1d130874603bce075c92d129f5d5ba5c5f83ee Rename readme.txt to readme.md
0c533674fe5fb4518b3296f8e950b7bd0e2aad07 Update readme.txt
8e34c16e1adfa2f544c87e1b3917f1ad624c7fbc Update readme.txt
c06b625e09a513334f5ba0f7e962be1b465fe389 Updated n2cms.config version to 1.1 and tweaked config source
563ed3144f3fde6166974c30b25b99b222160d01 Assembly version 2.5.3
07d8b9a930587275296ca9585d24b22a7b2d39df Refactored nuget config to use xdt transform method, now NoZip removes any zip vpp config which should make it possible for zip and nozip to coexist (fixes #259)
b3395881aa2354a71b5c450d46dacd01712b0e07 Correcly append edit=drag on links with hash (fixes #265)
191fdd067efe1d5410e088d4cb0edc3e5df846ef Merge pull request #264 from nimore/DisplayExtensions
f70b2e4614df172f7e362981f55c7ba4661f4766 Merge pull request #263 from nimore/EditableFileUpload
b25b519a3d10b24031eeaebced9f148e91391411 Strongly Typed DisplayExtensions
99f3050adb177055275c6dcdd3576218995d1468 Fixed a FormatException
46ee0731a14aadba8c3d080b4353190d62af0c65 Allow EditableFileUploadAttribute to be required
d8b700c143ae05f339e4e36fb5ad1a8a71e48f7b Added some diagnostic info about interface context
9340ef1177ba93fd2f9da7a65c792af4979504b1 Added configurable option to remove menu components via /n2/engine/interfacePlugins/remove (fixes #254)
46a3cd30e13438fec98250087952a26619fca2e5 Added some unit tests
c940c5f369996ba545be80ecbd10faf965b822c0 Exclude dinamico project file from nuget package
b0323a4ff70792c7d2ed2dd11c7d4d363856670e Config cleanup
7c17d4a589bb4d2ecfc0c3b81832f088e8c07e06 Automatic redirect when first installing
59800e94aa55fb6b7e99d864e81d4892361b2d9b Polished installation experience
bdeeba1351e9b80ba46c6edc7c64c585a2566b58 Changed folder initializer visibility and overridability (fixes #246)
c538546cee76d5c6e87005e36ffe4cc22aed0059 Assembly version 2.5.2.1
1f9b1675c1f4299e3a4c921a0a988932d29139b2 Fixed issue when creating directory
d8894474aa27698d3bcbf14f49f3628d3fc04f13 Branch info icons localization and tweaks
822b0b9f8b16c1e0bd9505ace7a6dfa443eb7d40 Fixd some links in the files management
3b8e914d86357941c7564d6e9402c72eb28644f9 Merge branch 'master' of github.com:n2cms/n2cms
2c4391538bb273d1a95f30e7d887f98e00b979dd Added icon indicators for various states (fixes #257, fixes #239)
81e67cda1ba2d2ac0ac0303afdcafc7ca826397c Tweaked version coloring
07a2a38f77fbdfdd44d9eb0a5ebc5df3f12157e8 Fixed an issue with csvimport
3e97c583d7bd8b80bfa93cdbdf896392016ecc79 Refactored inArray
53f7f4146e3c852a6a1ba904db98ff22c5711ba3 Fixed some IE8 issues
715c3e2c58c73606bfa527f242e1840339f4f1b8 Tweaked css for IE8
5d19cbae91896e1c8528c116345cc20c9cc75795 Merge branch 'master' of github.com:n2cms/n2cms
b5b0df10b70d520c90ab3e9df570cb4f23308bc3 Fixed submenu positioning after refactoring
e198a88e3548fa7fe0e244becaf628b645b634c9 Merge branch 'master' of github.com:n2cms/n2cms
f7e9a2f3fb1e532644f76d1de5efa9147e943ffb Made menu usable in IE8 (fixes #251)
ce292ac4ea35c4467392735a2c14e305e2d6a843 Allow MenuPart to optionally list hidden items.
9435beb039c7dc2816882db8f20d3bb942e7aafd Fixes #252, DatabaseFlavour values should be powers of two
34a818743763c7d0d13b3617fea479795b8951f0 Increment assembly version
ad42968ae216baf9ace8fedb102d74b3a5bf9085 Removed some js logging
02db487261fa8402936938c680c578c54fdc9449 Merge branch 'master' of github.com:n2cms/n2cms
3b34f72df04a93facb690ca0bf50ae64a4d58ed1 Fixed some unit tests
a574f9f3caca3d5d45a5028038c87d9e987f5b08 Show info even when displaying drafts
d5f09fb6164a8783cf9289f28b2913da051998d9 Add less-js (http://lesscss.org/) per #216
32dc6377fdcd68aa0b27aa34c00e987cc7ba79ce Update assembly version
17a5c5a8782e7a798da7035cc2b40f2e68a9a4b6 Resolves #250
db7a3fefe7f40978a5a42302bcc622273dcc9d28 Merge branch 'master' of github.com:n2cms/n2cms
c0c1729f6f95ff105b90cfae052b7ec6ba3acf15 Merge pull request #248 from markthiessen/fix-child-cache
61c140eb6569dfd4ad2d87903961568ca47e989d Updates cache utility with method for setting no-cache and modifies api ContentHandler to prevent client caching
e9c83fd07965a31f02cb2edd138d18a3704d334b Merge pull request #249 from markthiessen/fix-delete-ie
136508d6465cbd797e96f99047749f6dc00a0db0 Rename Content resource delete method to remove to ensure works in IE8
cabdc9f6e1fa3ef078a968c397d3c2f29b45ead1 Fixed control panel underlining
c3e26f12048f8e2889f9957c611653190f7a2332 Attempted to fix some nuget installation issues
ddea89874706bb9c7e6f0c38d74a8e2f8e0c1756 Support targeting for files, and possibility to disable targeting via config
4fd460e4a4ba37875b41552d91174de91f2be4a7 Fixes edit alignment issues, parts with null zone names, removes empty script includes, etc.
0838efe2686f4ef9a7d4339af90c89ed250b524f Assembly version 2.5.2
5d7009b0a28c392ce4d6cfd0d12424fe0d991221 Added mobile detection to webform templates
32341c6995b1b58422201cd3fe89c67965231ac1 Changed targeting to use template_target.ext rather than /target/template.ext
ff50793fb9acaa697236a0e25b81815abec5f5e8 Improved item list editor
86f27fc76a4a09a4ee528b8f9fd2ae5413bfad8c Redesigned item list editor
0c92c0a751d7934f5876d7cb63f25df457ef8035 Tweaked reusable parts
09397b7b534df514514c869470186c85085bab45 Link tracker style fix
ac0efff98eb8546a34979baf5359ebab4cf3a8be Style fix in control panel
36233da47c0e116ae7f84d1a7f24e40214da6beb Refactored part rendering MvcAdapter->PartsAdapter ZoneHlpers moved from N2.Extensions.dll -> N2.dll, fixed problem with actions when organizing
ed876c8b96c81be242b91b96132189219e8e20db Allow targets preview via query string
b01700d273fb6caa8cb952cd25095c69b7b2d744 Set view preference client-side without full refresh and allow adding query strings to all preview urls
5a45119868750092e0435b665eb70f86bd698d1f Refactored selected item query juggling
9ec96543e026efb666cc3105e748b82b0b8c8e32 Refactored css
dea877688b5ad9dd3a08b4256fb994e9a8c5ef0f Added redesign cheatsheet
d5a8d4663f2f7a2a323f9f9b8a73617e126a56d1 Tweaked flags for root, upload and trash
2f7a0d832aaa89b98efc5c57ebc04b8e46704936 Basic error handling
14b497ef926a777f1cd0574f825a76b45a0f113a Updated files
11e4226aae2cb5113d5fff433f186a2bc6dea723 Upgraded blueimp file upload
0b89d97db1dd526937a87acc72872486b0a00e28 Fixed edit parts from edit page (fixes #244)
4411feb2da4a2e961a630f44465e7640bdc83dec Enable selection in main menu
8d602efc2aef90814a09ea55c89e8ac50ca0c07c Merge branch 'master' of github.com:n2cms/n2cms
3381d48fa7d283038b8886ef470c6ec62e38bda3 Removed debug info
c5b304e12f7171fb3e7ad5001d88cc40d2115730 Renamed some partials
67666e44901a371a5f50fafefbfaa39a0a474ee4 Implemented support for selectable interface menu items
fb62df69ba5c6950f9bad720665a68ab9388dcc5 Merge pull request #243 from bherila/master
fe4cfcf2eccfb25fe74e413e8fc1e02057c0ae06 Polished zone area when editing
500d48c1b2e17115db2715dc764fade0eaae08ee Fixed help positioning on checkboxes (fixes #209)
ea8e9d783fca17afcab20562060396c0f0c05237 Fixed new item type selection (fixes #149)
79a8d679d29ce35761243ca6957b0f7efd6b8ce9 Reverted to sqlite db config
c30e98d8db0bad6aaaf5ad753ffb46bfb973a7b1 Removed syntax error in services.js (fixes #242)
74fbb04d9e9bd01ceea68b1c62bac856649f767a Style polishing
9949cdbabac6917f487bce5176c3e4a256415068 Polished parts editing
525b8c9ff885a803c369d0dac7de2712e18bee3f Moved context menu definition and added security option
2f99f21cd04d1ca1b679862fefcf95b8bd0a2c6f Merge branch 'master' of github.com:n2cms/n2cms
c8e0144d184af3cb9c982e3aa68b8c751510aa9a MVC support for target paths
6695e43dd523bc38ae0cb481980c44799c7dd2ab Webforms support for target paths
63957214e1e1d0da4e2fd318a02e3027da321140 Refactored targeting
628060080116db75c0ff3fdf1ff803464d668399 Removed tab scrollbar, widened summary editor, changed form layout and help (fixes #211)
a0a58edbc2222876968e0024d687662bed7fc2b0 Remove syntax error (fixes #242)
9c7986974c894d4ea723083320748082f232fe58 Don't use custom hostnames in the management tree when running on http://localhost (fixes #240)
04b5b93e0a33e3a2b40f30776f7e806ec855d8d9 Whitespace only
da71e53e73069a9ef3c9fe58597d6a0af5d115ea Whitespace and Resharper code issues.
78ad8236012de983362bbc9b4b6c0dc7e2f9de44 Add null reference check (fixes #226)
1e55ae74b708945aa435bcae0ed93026c1050aea Fixed move
3f078a932941df7db641681f87c9eb5fa7c904e8 Fixed some problems with the uploads view (fixes #229)
8b1a6534aea0b6492b18eed784835c770f88acda Record N2 version and installed features upon upgrade
b8e4626c6104d4990ff925e18589d96ed37c6df8 Fixed EditableDirectUrl to items in trash (#34211)
2c69deb01e1a3d984ea307fb87820b586ac16e5f Fixed nuget version tag
3007c55dd6d68614980467094bbec56c1eeae4b6 Beta label on nuget version
0000c6b6f7c9cf3453c7beee63318f57cdde86e7 Added ckeditor localized resources
ce95cb81cab4fa2144514aebc566f7c0506e8e05 Fallback to start page on content master pages and user controls (fixes #119)
6a88332b1f8781a6caf04dc078c14e989912f95f Changed some code to avoid index out of bounds (possibly fixes #201, please test it)
172adca07485b18304111cb4078811264fb96c23 Refactorings
bbdb94493ba2c307c72ceae512dbf1a592ab1277 Organize polishing
31cd9c3c628579395ebe15c5786fb95c2312b582 Increased assembly version
dc99b84265c5907b3326839aded5aee56d6dd8ae Prevent flickering
3988dc6fdc930c4ec6cc0a74a39b47f0f41e12b0 Merge branch 'master' of github.com:n2cms/n2cms
1a613bd5b9b0a93cabe4cafe31b91db691621543 Drag'n'drop polishing
7898e63ca60e4a664970c5711a2f9e7dd7415492 Removed dialog interface when editing parts
a3cf3256388cffdd3d96d3b5d1f41b5cfe43721e Fixed calling control panel actions
a9d66a36d366d32d31b9ef8b22fc1262f3f6673c Improved flow when organizing
1e32f90b5076b8a291f8e85fef53cbc34da3f5e5 Moved some frame interaction to the frame
796bb373a7256d5d7fab17f458017d5a8d90dba6 Always organize in framed management
8803c8ad479e7f8de8fa3e157b1695936dc3aa24 Merge pull request #227 from alhardy/master
033982a4dc04cb9fc2674d8f1069f3f0b3b04b55 Fixed mime type for .woff files
29ce24f381fe2dc32070fc16d3d6a4a999ca0497 Fixing solution files issues for successful build
eea48bcaed59e5554a635c327eaedf3c29bd641d Added targeting base
9358617e7f6f50fa3aa04ff8af81184b461b730d Modified package dependency and names to simplify upgrade and install
aa086ada0f45909a87a1c879d3a53aa1be4e771c Moved parts from N2.Extensions to N2.ReusableParts
0add22793c8dabdf409eb7f32ac899453d54e345 Fixed font sprite when using zipped management
92f6d9ccecd73f0a777327fb186ab6f1140c2872 Reduced js logging
1e5b8407d156849992666bd84764dfff2882f72f Added some file types for vpp file handler
33594699aac2fbcd2e8409c8c9627b080a3f46b9 Fixed icon handler with zipped management
b1475387e9b0f4b3ac59ef416dc83b58b0e9cbea Restrcutured solution items
4f161a88930e4d570b3ead345763f9f211eceda9 Some IE fixes
655f9fa72aac90f07e1024577ceba2e57883ef7d Some IE fixes
ea275f91d11000eb89a9e4e1b7728ede56318eb2 Fixed jquery ui path
47dd1836344bc5ec37fa597ece7b35d1ca37a40e Referenced minified jquery ui
16d3bbde5d5a1f1985ec5ab4035213cedead5a44 Prettified installation screen
c157b1279208bb0c41d4025d440a91255db71d8c Reduced output size
1f0d9740c907ae36dc399b5679c3b57797f03495 Reduced output files
954fef48699d4e60dee36e6c5be46e29c0adfd37 Moved localization to separate nuget
51489547a476015f9909909c26fb738b4eea28ec Allow extending translations
b3f91bc8117dfce3c67f0c1a696b41f24b1dead7 Fixed js error
30e1bbb4fcb16d08bbb74e6e0b689637bb6c3f5c Allow missing configuration sections
17c07fedcbf23eb54f5407a14657074c5458e105 Management module discovery
7dce2d2a4bff8826965f5b8a55fd7bf45805e4a4 resize directive
c3be4a14cb80468a96525b98f3135397ff3f0de8 Prevent scenario with infinite context reloads
ca7fe3bbb23c36ec5830794172b0ca91c6c2de86 Merge branch 'master' of github.com:n2cms/n2cms
2a917d6055b0bc3f322b8ff7f4d7eeb3a6cefa1d Fixed preview and splitter
ad612db2ba240ca514bfff086c2cbe3d8fa42c29 Renamed partials and allowed partial override
8872fc7d55f6b0d5fc2a069ddaad708d07658ed7 Enable divider on sub-menu
52e973b218ec4632a99f1b99886be67580a34877 Merge pull request #224 from GrimaceOfDespair/patch-1
a13c0b73fceef898a2b0cf451effe5ead00d8b23 Enable sub-navigation
19f8a6d7cce0fd30c599c9d90816d05178a07bcb Update ThemeViewEngine.cs
77df6fedfe52992197af00587648cd2eacddfe04 Fixed gripper state coloring
1dad16c374100d2144d9943e011ca5f8541992dc Increased sidebar opener size (fixes #210)
380cc1aadd9891a6cee380d0c45ce6dc82ecf8f6 Max-width on dropdown menu (fixes #196)
56a646042ef4d94bcd8bb205f092dbfcac9ecd09 Prettiefied framed editor (fixes #212)
f6cae5e4de823a83922935c919ec4d43db99ed38 Made some navigator methods virtual (fixes #223)
b433111693ed7c3c7c9b974174f6b78b7eb4953b Refresh context after (un)publishing items (fixes #214)
d94761d3fa73af6b42714b2a3eb64a22b59d34b8 Conformed control panel to draft/unpublished color scheme (fixes #gs)
e70eda5a3edc230aec7b5116ecf046ef0ae6fea2 Improved versions menu (fixes #gs)
93037754cedf53e79a36fcb22597c429f464c985 Tweaked unpublished color (fixes #217)
ff1c684b8aed7ff50eca1202d9d699b0466e7145 Handled exception saving assembly cache (fixes #219)
8af84ff1858b05c50dd0a4484bd86c92500a642a Fixed issue with language flags sprite conflicting with control panel
1ce004676a4aaea5755346ca99f7f00daf0df347 Fixed some merge issues
c42181b2a0b536dafcf58bd21c59631c9f4c39a8 Merge branch 'master' of github.com:n2cms/n2cms
60a7f83a121cd79272e57ba45c314f4a0f83fb2c Removed debug panel
cb893654811ef2deabb7e798ae840eaf1cb73584 Fixed context discovery issue
d783e119c97b728dc488c178d10247c093c78d10 Type info in UI
a5253942442a786cb3fc18053fa2ceab8b738c0e Fixed some draft selection scenarios
c241a9e8db83fd2508ad812f9cbc44eec4e0879f Fixed broken icon link
3777d129bc9a7a9e642b2d2d860848592c4a6b87 Remove dependency on Myriad Pro font - Resolves n2cms/n2cms#215
cd760385984d8030ad80490352cbb7a8f6137841 Updated .gitignore
3aaea30eee3e9cc7866385f37157d991e5e78713 Removed some other stuff I shouldn't have added
789baa4d871c6452fa47081f841e647978fa2141 Added flag.css to ~/N2/Default.aspx
ed2a77f6b174606227887561968ed61aff8700f7 Removed some stuff I shouldn't have added (sorry)
cf24a5c0e091b93acbc6622faf7918d799f0826e Merge branch 'master' of github.com:n2cms/n2cms
86bf14d9cef47d7a166aa8a813d7cb26571d2801 Removed obsolete, misspelled Constnats function per n2cms/n2cms#200
2a6721051ec6fa9abb97fd3e19c0f9ce0ff0362c More whitespace
5b8cabe2b859d19e82c182af3f61fdbf447e7dfc Whitespace only
28f59c6ee467dd33b9fff74ca825e3ab121b5439 Marked conflicting "cf" CSS class.
b53626c56910a1484ee30aef62d60bf48afd7352 Removed FlagUrl
ec2d0c45916fc447a1707abb2bbbf1e4c8455f5c Remove dependency on FlagUrl
e56bf9dff3fb37646c02e521dee7c89ccdbdd87c Fixed a bunch of bugs in MenuPart; behavior is more as would be expected now
c73977d3da292f1f10bb3ec28b747ad0f4cf8d65 Increment assembly version to 2.5.0.11
ef78ffb3a9ea351bcdc25c58c9ae47008ede9c27 Fixed some css lineup (fixes #218)
aa3c7cf6b20388889874f21c697f327b369b2f9c Tweaked gripper
3e63b3953a073523b1f822dd0102191c700e87e3 Localized navigation
2e90e00fa84173eb7ba15a74f351f417304abfb7 i18n
2235cd5e3b09d9c2035dd9e971f907c98df3e5eb Display parts in navgation
a582b75ba16fc71a1bd48cdb2db43f69bbe28b5e Changed icons to font sprite
1b186e97d2b54ada3c12a5079d46adfe80b224d5 Merge branch 'master' of github.com:n2cms/n2cms
d3b6441b2e581640121f062442784d9201312dd6 Reorganized navigation again
c3693a8240093af172b4524731c13c7d75494034 Reorganized navigation
f52d6461c6f37eac18478cbaebb1205e9c1090cd Tweaked search box style
6eba8c9c00239acd5b49c14ffb3a5324adf2778e Made some methods public
7b58d4b512fd2ecf481eed7e910d4eca06cd4170 Merge branch 'master' of github.com:n2cms/n2cms
7b9d3eea9c8f1ae383deba938eade5c28f886d0a Search
78f59715b60aa01c43b7251431c36e5cc5bd5109 Merge pull request #194 from johants/master
3d7e7650eb623cdb85942e3fe93a8a48ec44d978 Merge branch 'master' of github.com:n2cms/n2cms
187d49afaa17f595b05e0970bc23e52a72553153 Update libraries
818946fdf80817db0b074d1af949ed59bf3e0bff Iconified some things
b61bbea75c4c20fcab60ca8f7d57276c59dff11a fixed js issue
9871f318a4b8ff2040634479e506a82a0a489bb9 Merged redesign -> msater-redesign
a2ac994c69096e8607760097dfc614572a82dbd3 Control panel icons
1f43a086df19e6d2a542ccb2d90a35eb988fdccd Tweaked icons
641d3385c358ce375c9d80f24b54b582245595c5 Improved navigation and context menu
e80f63013d315ef09b10d6a9c7220a0062882efe Interface/Context api injection points
cb1606cb3308d206cce7de96cf65efae7de9d4a6 Upgrade & fix screens
44896f678ccb4d389f15d3b6e630309e0e3c0d4b Installation screen
7f1c170d508a80b39f47727af97dd70386d62d49 Login screen
9e82bf637227e4b85dec88e19ae1ff3e61a074b0 Moved api handlers into N2.dll
7ac53222ac6a6973889b90501a53ff875b2989ab Service consolidation
25c79111c1a4831c182c78795682d8f1c58fc3d0 Consolidated some API:s
c76f059d4ebec0ab86b48b6892eef9b0a1b28ab1 Refactorings, show main menu when managing users
d19d6110e4e914ebf881df1341dc32f515526179 Refactoring
54bd8f3a90f321b5adbdb95e713c3d136104adae Some content cleanup
dc10155b38690f8f67776c4c5e61334977db2005 Fixed actions remaining after leaving mgmt frame
03f8146bb59ec6e8dd14c89875ea82ca0ee706ef Less options from edit page
9d800b3d6a9ac974dcafe2e81590144660d4a8b3 Scheduled publishing
12fdc0814a72abc2806493dcac9f77fb597bdcda Future publish api
56a914bc42bc80f8359b16f98e46322f60268b82 Fixes
7b17f5e5be3fea64a3aa8f99c0580207590d729b Some selection fixes
4b4107cc397a233a1fd1ce738ddbfc24221fe6cb Some refresh improvements, security checks
3b7b24bfb6ff73bf5f307a99aaba90ae782549e4 Some workflow buttons
c7f8c15814e53e72fca9720271ca5d1ef37b618c Typo fix
80064844ddf9089770d865d26fcc9c005645a396 Validation bootstrap styles
0b30954ffc89969987cd219cc8decbdeef97a090 Security page info
6ef82192946b368c67eaf0f90d7a9ee23b57e3a7 Improved legacy refresh
e57c84ae1665fd5c44205326434b56ab4dcf56f8 Improved info box
f80d168c61fa769187fecccc01081bd560258a7a Publish state improvements
e9daacf398c9f8cc32218f1243223da4f5f555aa Framed css tweaks
553799367be3b9c57f1e4f788ac4f92a3ecaad5d Improved legacy mode
bab5bdf76528d6399809074e52a570864781ea08 Avoid toolbar flickering
860fe7cb7271350512f47dcc6e29c35b52747e07 Removed some label classes
fe460a86ecaa87762589f0e81bf59699e4fa576a Prettyfied label
7c7dac345cf8b2020e46da038cffbbbc538b7f8d Fixed framed tables
5265154eaab0bc457e33924566e0f6d6515ed027 Introduced bootstrap to framed interfaces
10314017af763cb403ca641ae8d9a623c95b4398 Added missing menu options
22ed704fcd35a189ca7d82de4d8ddd64f484be57 Switch to new UI, with legacy config switch
4bfc2d9c71dd84f8550c18a14c660e64ca16dbee Jquery once
7f82f1f75bd552799e2b04f449eb586afc3f43da Hide duplicate control panel options
f656ff309f0b3da4ad8de33da3148dec86ec91a9 Refactorings
4b96e4d6093bd8bad9de2c17f6cd40bfc0422ba3 Adjusted icon positions
0da4c36fd2a507ee092646b248f71db77d78d0f5 Fixed some styles and globalization link
ca44d456e0a800c7739f66da54267fdcf768256c Fix empty actions on trash
47a447f4d07b3aa8765c1a0e0a87a16990076470 Toolbar improvements
141b838af5ea1fb883b977b79035f0419a1fc629 Font-icons
ef4a329c1bdde5ab5f00afe357aa5bf8d31cf1d8 Fixed selectedUrl with version index
aab1431724357d1116f886665d5a07ca89f1691d Hide toolbar when actions handled from the outside
0337fa827261b4e6151af40342883e8e110ee64e src/Mvc/MvcTemplates/App_Data/n2.sqlite.db
48441977647cf344f50bed94a6736f9dfb208599 Fixed some issues related to context
eab9e357b488a29f9a91ad879e3e3be8ba1940c2 Introduce n2hasher utility; Resolves #188
133bf6a71eced91fc88d28e3022a7f8e9dad83cb Resolves #104
39523d77f72bee0eb906063ff62b8aa993b3d1fe Make everything use N2.Utility.CurrentTime, resolves n2cms/n2cms#101
930e805c7c23fca1f2da06c93626abb06e1ee1e5 Dispose bitmaps properly - resolves n2cms/n2cms#95
f3150af4bf0d6df331bfa518b70d83c54cbc1146 Hack into frame
e83935de6aa6db953421eb00bc01a4ca775416ff (whitespace)
68df0f6d777839fecc080092f0c02f5e02aa70e8 Fixed action bar refresh after closing
56904461b5f2e5f380b761b6322c09d158e82a10 Add template wpp.targets file, resolves n2cms/n2cms#151
da2abe01e76ba182342b09063f9d702668084845 v2.5.0.10
c4c7bb0253330fcfa1a3d8b4ccc3ed4b2e6e2412 Increment assembly version, remove N2.Search.Lucene reference to get the project compiling again.
aa7ee6b2479902f2fcb90a55b383d13711dacd8a Tweaks
7a66df3db963e733bdaf9be505de9f4a4b0f1ea9 Context aware action bar
f7439a619639325a49470be91c3d316d6999b73c Some preparations for framed interfaces
d49ca49f6db7e7ffd8684d9df8cde013cc10bf6f Remove obsolete SubNavigation
1d330593fa5b188fb6e2c22302761f31a666c80f added SubNavigation component
d8cecc115df4b32b7f5759b11cb3f068b040aaf4 cherry picked nothing
8f70327ecf448f0a6484b248e7e0b26c28ec2007 Add MenuPart to N2.Extensions.csproj
456ce7bc2e4c60b16f8c414d77a48913054e19ed Merge branch 'master' of github.com:n2cms/n2cms
4a2b69ea8eb833c7c5373b028ff98145a41d3dd4 Add MenuPart
d1ab234c3fdaef07d8fd8ebb04de32b4c650df23 Update assembly version to 2.5.0.8
7f61979026812e6ae1baf39fec00ab690a0ed034 Normalize whitespace and improve a few comments.
92b381d0982f92573e030f86b02d7c1e2353d929 Fix the issue with GetDataItemsByIds method in EditableItemSelectionAttribute.
ba0e54ea35820bdfeea0698695674ae696c130a4 Fixed context menu issue
adb99ead13fa5ce3197e8716b9faa2b17af0d586 Refactorings
dfdd5141772a85c1afb7ab8c4eec9df8bad6d256 IE tweaks
cba360a6532da47f1599ec82de17eb627f40ea04 UI security filtering
4be33659ede86fb81f0a690f676f86c15aff119a Refactored context menu
cb1c8ffbc62cf1e5a486a3a24dbb68f56d939c2d Add
1090a3b5a10099be7230eef65d8fc1bddb5dd0ba Improved versions on action bar
ef53179acae09f4361af7bc4a5658a89d9a8aa91 More coherent states
b84bbc24bc723bd804edbd086abf943e528942fe Improving view preference
1bf3807e68e9245adedd24546a3892ea57431eb0 Tweaks
0e4ada020233c62a9cdcd947dfb5bd9f41694e7c Refactored action menu
4469580fcdd9c91f7576f07c3d2c8a4479ed6ce9 Merge pull request #189 from DejanMilicic/master
d377b4347be14c4051811c0e5cbebcbaedf1d655 Search from the box
d02e85727150f89d5b84cc74f89b59a4b1052c58 Fixed logout
b951b2727bc75e508c9e1f18361c3adfd6944112 Enable links on main menu
32190c24bce337e58f5147783d451b84de330d6b Node selction when navigating site
1923e6829b2a3074031ad2757200e8773f6cc2ab Refactored tree
79adfc806acf890e5ae5eea0581537e644d42235 Polishing
463cd75bb1ff5d4cf719bdf12dbe7f94f823400f Alerts
b0b0c1ef44c797f8be74b1a8536a887d7559e309 Server-side implementation of slug generation and validation
7db95cfeb3531c598b1cd019a1b68e6097ad165c Merge branch 'master' into redesign
883e28d5087c86aafd17dd6450f5b71bcccf034f Modified iconhandler to work when no handler was mapped (a.k.a. my machine
275f254406c647da9d29c9f49d210cc2d48ff08c Removed xml storage from branch
6b1c3748b22ca896537c89a4a1a00a611aa5751e Exclude missing file from project
3dc25581771a37772d78a5b74c394c407ff7292c Merge remote-tracking branch 'bherila/redesign' into redesign
29f0ab8adb5649b502e915f12160b70750bc5f0c Removed some commented code
79bb19a6a1ed2d2afc17789eaa8b50958d79e05d Merge branch 'master' into redesign
8dde56934ddcbcf1474e5374233c2abaa8daab84 Merge branch 'redesign' of github.com:n2cms/n2cms into redesign
6ea524be34bc89b85ff584655dce9483d2aedcba Reworked sortable tree
4bb48ce8be0ca6d43072bc4479a5ee196e3959a1 EditableFileUpload whitespace fix
bb70ce61b98b586389cde88ace42421cc1b1e842 Merge pull request #181 from andy4711/master
151bea3b4a1718ab1c8c431c42ad401cb4bf634c WhiteSpace Problem
d022690b4fec2e4400d4e08e78f7c80785a83d7a EditableFileUpload WhiteSpace Problem
7aab69a10e38664b99505daa24cf936fc938fd6d EditableFileUpload WhiteSpace Problem
8bf72111647f3fdfc0c62a2c1a1989d67026959c Merge branch 'xml-storage' into redesign
1d1db03f49e6fa05eeef6e3d14e298aa9f385ac1 Fix a bug in FileList
620ab525a4d6663cc3b309df41add01fe458423e Refactor a few things
0568dfbfa1136d697404ddcc58ae192f9e85908a Resolves #178 by using IDependencyInjector to get the URL properly
4ba4bd1f6ed68bb9b12fed9233dbd83a3c8f5528 Improvement for EditableFileUploadAttribute
eff32d54766f30a8b783700b826cb0e81a7452bb Implemented load remaining nodes on large page branches
71a3db583c198ae5843d7e78336566d597659921 Remove use of dynamic Content.Data to enable static rendering during serialization process.
e58d871346f54fd697f0aa38f8a047322364f050 builder.GetChildren(null) returns null
0504c26740439c5270b961b3c8dac2c78e7d36a3 DocViewerUtility can now be rendered even if the site doesn't have a start page.
c634ced8f423a90760d2800b40b241ba66690648 Don't throw an exception when property is not found. Return null instead.
e40f533e88ffb3c109230e00d7389c6e9cbffee6 Resolves n2cms/n2cms#176 Non-page Content Items are written out to current working directory rather than the data directory
afc7bd2439070295728a2d8f2875e150aecf6dcf n2cms/n2cms#174 proposed fix to NullReferenceException that occurs when trying to import Dinamico -> XML
5694af29fbeeb3f24ac684864083e443e7c6c5b5 Make only pages get written out as files. Attributes get written out as sub-items within the page.
e76ba05c3610fd1c8ab8cf101b73767bbe39749e Merge pull request #157 from bherila/master
ad9d8c597302950e600d3738db3b9cbc7a02ecff Merge pull request #169 from andy4711/master
a3f2c02986d39618cdd0eedb7919317d6c4b3c13 Merge pull request #167 from bherila/redesign
0e8c9d9bc0f3ce7453c0c4bb99ae8e87f591baff EditableDateAttribute
7ce667ce4247647da0c7f91e77ee9555f5635cf1 Try fixing a bunch of stuff. Import is still broken.
46eb1f5c6b4a0222f284f97a30d18f0e4bd0a958 merge stash
5100927a431d56bd2000b87384f27b8abd1bf1cd Increment assembly version; add gitignore
827af3a949e55f04da5a2bd355cfb9dbd4617bc2 Merge branch 'master' into xml-storage
7f202a1cd9a0121e5ecb9be071b2ef59ad0d81ff Added Thumbs.db to .gitignore
3ad34d5f73cc9b7558b9a70ad98e2c97955709d6 Added glyphicons sprite
9fec3fa1f2859b4571eff25e005e3550f4109132 removed thumbs.db
c36f1dee9280a3400c6445fe7490dd30678db623 Merge branch 'master' into redesign
85003cfc3f17088aaa8604dde7eff77764649d1f Added installer code
bdef92443f96719fa30adaa79bb81d451d41d402 <whitespace>
dbf77747cff52fbba71bf3f07cf8f7266502bd38 Update schema and assembly version
fbf4dd81b03de7db312560e7da0e4ae673484bf8 bugfix - DLR can't be used to determine function overload
78177af3d1b3d2a48d550b5deeb9c36cd79b783f <whitespace>
8c4541c9d746bc5e8523b240a3af58bf3a45d285 Delete individual flag images
e085d6b5b3560ec34be74fe3cfd72bfc75d63ef6 Add optimized flags sprite.
8609a9e0cf03b42c5d366c5ee10e15224904402c Made icon handling SUPER un-intrusive.
441011dac9954b72ddee98f2fe704474a3ed8985 <whitespace>
91f02d6816468c9271f546333c94e609f8516eba added TODO item
d8efd2ebb29e192f2a978735aa9e8a10067d443a Added installation readme
038ed979be8404b177368da4380f78a5082cc566 Remove silk icons
94fc6e7f3ea27e434343b89497a386f893295ad6 Add silk icon sprites
c8912f1b86fc5323805e795d11109ca77da8ba75 added IconHandler
00371589dc4a9b4f57a3a941fe8c6ca6b649918f Add DocViewer option to FileList
ed6192f24ef8404427d19c2b8a9c0292a13f5844 Frame cover to allow resizing with iframe
897a932ecf3d9031f70aa8774ef61cdd989c2cdb D&d move and sort from the page tree
b7c547bd37ddcb8d590b23566263da6b950cac58 Upgraded jquery to 1.9.1 and jquery.ui to 1.10.2
8fe223c1a4b540f3110dfe83feca37ec39ce3b52 View preference
76d4a57af9b437f29c0cd5ac977492f57a70bfea Language context information
e4f1b2e52e2dbb616770476a99d85c9191c977d2 Page language dropdown
2eaeaf241d65148b72a04b6f24894cd8718093b4 Page action bar
b7500030e18cc6f81eabd32866cbf62a907f138b Allow expression in page action items
bfbe275f910b2a7f58d94b2e6cf8990db77c17dd (XML) Implemented some more save/load logic
960f10061a3c2a6391a2dde3351e011d12846029 Fix a NullReferenceException and add NumericHashtable for efficient lookups by integral IDs.
c0c270004d64d890ee9d00afb462c973e29aa99a Merge branch 'master' of github.com:n2cms/n2cms into xml-storage
ccc0c8f266f3425bcea5d7dc36ce21f5a9dd47ff Added uploadFilenameStrip attribute to config schema.
0fb3342e0241911f0877c787291fa45edd6a53a3 Redesign selection
9ffe69e2569c7624c7364bd3658a64a3743fd052 Redesign icons
d2115df385089c0a6d97d0df0e6c1b109c16aea4 Applied Fredrik Gjrdmans patch which applies caching for dbfs requests
19e5417258f0d8dfd69ad75831744046697632c6 Expandable tree state
c414378a8d21ba2c1013b32a4365d9bf7704a684 n2cms/n2cms#162 Refactor XML ContentItem stuff into generic Object repository
ecdad41dd8c043740ba9bbebe00bd6a3cdef666e n2cms/n2cms#162 more work on HtmlContentRepository
2d91b09fb0f4ae487968422be1369cacf6efd010 n2cms/n2cms#162 remove dependency on UrlParser instance
b6405fd36fee25437fd5c60f86952eea01b8c6d3 (undo recent change) This needed to be IEnumerable after all in order to work with IoC
827a03d20db65d50fe3d5039b4fceaf794e5e027 n2cms/n2cms#162 fix up Xml database flavour
a1f2333e009f105e4a8cdc7b32910b8ecbc24252 Code cleanup/resharper
b1a96c6192b393f453e489c81fd71ae4c22b500e Whitespace
63561830f520330c428da015fd842d007205274c Refactored out ITemplateAggreagor from IDefinitionManager to simplify dependencies in several scenarios
b1f93cdf70bdae4c39c9d4fc856cd990b0c65429 Merge branch 'master' into redesign
e52626b808276a4abcb2229d1fee766f84147c11 Redesign...
f19ea39d6f9adb15834c99f2f35b79968209496f [broken state] Committing progress as a back-up before switching to Windsor for debugging
aafe38a5cbb89b085d9a0395d88616b563669b83 Merged master -> redesign
024c6fb9e43d59aaefb534ce6d8ab367d89ec0d5 More UI
1fdba89f9d6867f43551a03d435ada3ed6a56eda Add Xml flavour to the configuration options
16af9eba26d51a3e3086cc627d2e1121ce1f3e87 Work on xml/html reading/writing tests
c1ef32ab2c51336c895ed3ce1d1f076a40bc50b0 Work on HtmlContentRepository
16f4dd2a5dff8e1ab738a32d41b53984a3afa73b Introduce IItemXmlReader/IItemXmlWriter interfaces.
4951a82b9e50aa81a32337d970297fcac0bbd0c3 Merge branch 'master' into xml-storage
3575cc1b82f2548c93fafa5010caeb3e4060540b Added XML content repository
8dea6e41b5769ed7832bbda0bbece5b0283cd322 Include angular js
64407f10bb82827159dc7468dd4eac1a793f81d3 Fix whitespace
e7de940feab8dcd30c4cdd21c832467c424d41c0 james, please be so kind and test changes I introduced: GetSitesSource - root content items doesn't have ISiteSource GetPreferredUrl - null ISiteSource case, null or empty site.Authority cases, support for Site.HttpsRequired proposed (TODO) ApplicationInstance_AcquireRequestState Don't redirect N2 default.aspx ASP.NET helper (will create an endless loop) Regards, JH
809b6628ef17650308fa6bcd803698c774f1cfc9 Added support for consistent URL enforcement.
2b07d056b4a10d42e7e2d09d40fb803f0ea3ee51 Temporary redesign files
a8921434c9a62432e1699517acc873fe59995879 Merge branch 'mongodb' of https://github.com/n2cms/n2cms
```


# 2.5

Content-source API for delivering content to the management. Support for moving search index to separate machine.

```
e41cf9f66d6fb677205023e89e4cf8231f98f18f Increment version
c3c42624caa9e9d1c45d619c9923607045f58994 Componentize up nuget installation options (zip deployment is now optional)
16434c45539ab17a6f5f26ed82a41afac5d0e2ff Merge branch 'master' of github.com:n2cms/n2cms
356fd83f56996709d075f1194cb94fb71924d8e1 Fixed problem with doubly-encoded content on parts with EmbeddedParts
d1cf8b3a58cc29385d97cf6f0f180ec0e974dfe2 Refactored out ITemplateAggreagor from IDefinitionManager to simplify dependencies in several scenarios
272973bcf17d61213d3fc9b5f7723294ab02ca2c Merge branch 'n2cms-2.5'
523b5f74164724d34ef1a06d5bd8e71540099d3e Changes to lucene indexing that might improve stability during IIS recycle
6d4d9d134a98c2e60124f7da40ec174e9e580433 Merged andy4711/master -> imageresizemode
f4b8c050a579d8fcbf2236469ea8457b590f6a38 merged bherila/master -> bugfixes_april
e870e4588a51eff298a25b87b4abd6c91ea0e359 Support multiple indexes using index server
1c363a6832e796f4b671e360ff5dc6395e872f05 n2cms/n2cms#156 partial: Nicer exception when attempting to read versions
1d2ac65565fd3da755adb2e8245a5abf41556eb2 Resolves n2cms/n2cms#155 by fixing path slash directions in URLs. Also makes paths more consistent per n2cms/n2cms#28
a460d7af800cb46213d37a7bc623166d6d8d4a1d Resolves n2cms/n2cms#154
3f30f8c1dc651a10df90b083614259bd96f66ee1 Resolves n2cms/n2cms#153
6310b00bbea4d95392e9995754f7ad191cba87ac Resolves bherila/n2cms#3 by handling exception cases gracefully
45dd0c6396ef72fd7e050947e83441065c35330b Resolves n2cms/n2cms#154
b680fb9a3758ef384b235f54f2de8702416ee25c Resolves n2cms/n2cms#153
acd4966d872972448d13330071ca61e83aaa16fb Deployment wrapup
2d0028f7f4a1db8dd2a8e691925a7410b8b402f1 Resolves bherila/n2cms#3 by handling exception cases gracefully
ef311972468a2c91dacfbea0ef0dccc385f22962 Resolve bherila/n2cms#8 by wrapping bootstrap grid in appropriate CSS
f3274ee6459fc428858aebffa72737da4f31ee91 Resolves n2cms#123, now displays 404 or install wizard upon broken app_host.config (rather than cast exception)
5f64de38f9fd023acc1da7abcb71a9e3546dfc10 Resolves bherila/n2cms#4
59e8b8f21dba41e166c817077197815e1a73bb70 (Merge) Increment assembly version
bf797567306dc5a4d3670e3a528d7d8371d3b211 Increment assembly version
b0f9cf6e421a11d4c53eb6c299c5fa141b9dcdcd Fixes n2cms/n2cms#129 N2.Web.Link.To should htmlencode ampersands
5c9e1bc0c94affd6eeea7efc6411e3e2fa5400af Resolves bherila/n2cms#4
e0eccce2be36103221b040d73968f9596fa1b6fa Windows service support for n2 remote search
e9edf9e86c19cb1b459f0c8776806c2481771d97 Resolves n2cms/n2cms#133 MVC Posted Form Data is null
b55eeebccacf55861057cfd2c59c923b3b56d3e7 Fix a bunch of FxCop issues.
976047f8d12fc671b710f45f1fca8c1d24cd5c1a Resolves n2cms#123, now displays 404 or install wizard upon broken app_host.config (rather than cast exception)
8eaeed2cc9ca83862beb14f225ccd8d3d46e62f0 Resolves n2cms/n2cms#133 MVC Posted Form Data is null
2d45bc39327172fa8dbae34ba8370cdb5c57e5bb Fix a bunch of FxCop issues.
48b151af70989dfe735c4ffeb6c0b7a1c9705de3 Remote Search nuget
a3c82b448614a010347a9c14398cb336178b384e Added MongoDB nuget package
e9e70bb569ce75c38c359b61e692f14aab46ca56 Resolves n2cms/n2cms#141 with a comment about User.IsAuthorized
b5bd22b02606bc37e00c277fceafe498080899a6 Resolves n2cms/n2cms#141 with a comment about User.IsAuthorized
2f23acb22eac07b91df45f1ecb74234e7dba9559 Fixed sort field serialization for remote search
f40cdd4920ddab861f367d52f6a47a8d945e7bd4 Added support for shared secret comparison when using index server
62254d7213d10a447976735690e7f350595f4afe Removed double close con ctrl-c
8c7b6f8cd0dfedd52bf93e4d7c512bf6f4eb8d66 cls support
e603e3ab96c34a865bda2b672515e2a88443bfb3 Timeout configuration options
9cf478ddbee31ca15271da084e110a31d5131832 Refactored remote search index
ef6bb81f79c1b2cf4f8c4b68c0b9af5e176501fa Re-used lucene unit tests for remote server
3090fe22e6f4a8957c89993cd9bb6de137db2222 Implemented search server exe with client
dd60da3e5c63e1a18140135e82d08e2c24021438 Moved content change tracker into N2.dll
08a249f296d63e4d4841c663e8f1f8dad1552c57 Refactored content indexer into N2.dll
03c85bb433cda23e5d7b1be8751b2d5807669191 Resolved null problem for deleted indexed content
93d43a38efa49fe56e97c82678be8576e0928f37 Refactored lucene search API to support search client
380f5bbf57c137e3e94429998233d20185b238ee Refactored into separate lucene assembly
a67e971cee63cba30e1204524faf3592b340d63b Resolves bherila/n2cms#5 - add icon to redirect page
5497150527862c98c801dbd0a34f7dfc088dd6d9 Resolves bherila/n2cms#5 - add icon to redirect page
a6dc47f6f682accf3247729ce4deb39bba53afce Resolves https://github.com/n2cms/n2cms/pull/87
f2354e3076c1ab32748bfc31d510477bb776b165 Fixes n2cms/n2cms#117 - Image gallery supports sites in virtual directories
5b564b48519ce38a3088cb4ffd11ec7502e5badc fixed n2cms/n2cms#114 - Login + ReturnURL fix
4d0cbe551a220aff9956c0530c13cf1907ad7ce6 Fancybox bugfixes
a9cd20049739fa044177eed7a56dcc19a1d06899 fixed n2cms/n2cms#114 - Login + ReturnURL fix
61cd51134c419935e9615ab3c561aed2dda6fef0 Convert enums migration2
2731983d14b23e95c941aa2a5605e2eadbf27187 Merged mongodb -> master
5fcd60b671c58491e379b8683c243b72c2de222a Merged ckeditor -> master
41e2ea0d97742b8b564e5af811c361ddd7999cd2 Merged origin -> local
da88ac8a16254b1134a4832cae1bcb0d91676574 todo update
65bcdc8f327fc6d5cbfb2093895f238826a582fc Enable slideshow templates and fix some errors in the Fancybox extension.
c2885557470b81996177e3bb8d4a671fce0ca5c8 Fixed miscellaneous whitespace
85c0c53aa2f21469a98a3e05583f81d93d7a41b0 2.4.12.23 Added support for Fancybox
425b2564ce6598101122c22852b156452a4150c2 Merge branch 'master' of https://github.com/bherila/n2cms into n2cms-2.5
5d05fec22262d7fe9b387f9fb4c0613bee352f0f Merge pull request #85 from bherila/n2cms-2.5
f1d96cf7172e361ba1dcd37c0de6bedcd6dd8391 Merge pull request #86 from bherila/master
9ca0c0289012b66bf0a323a4d2b530c11da3b1b5 updated todo
41a77498628417fba9841d6a29b4119b912b5872 Additional ImageResize mode
aa41fa6c3b002130bb58256ddcb9f3e3bb698976 Added ability to look in AppSettings for n2.logout.url and process this during the logout from the management interface.
4b75ee04dd7547e45d43eef05924951ce46068cd Fixed tabs
c1aa4d98118e3e2c01775cebf99e221e69830ef9 Bootstrap source is now configurable location and outdated version no longer in-box. Default location points to cdnjs. CKEditor will accept and preview Twitter Bootstrap styles by default.
5ea0f428e64750d58432bdd3d3bae33878a325cc Updated FileList to enable reverse sorting. Increment version to 2.4.12.22.
e8719aaf7e6c4c6f10c55da446d0d4a247f44be4 Updated FileList to enable reverse sorting. Increment version to 2.4.12.22.
9e34efd3f34c079a2016f92f43e486788290c456 Updated SQLite to 1.0.84.0
6c161dad10293269faac2c46c99a559c3de7e8ae Increment assembly version to 2.4.12.21.
214f53a227406f048df8c7047ebc53585cfd0ec3 bugfix in loginform
0c5946a8bb61a20575feb4b5a49eb1dd9310c857 bugfix in loginform
cad105cec5ff5c70e70a802761854a6cc812f78b Increment assembly version to 2.4.12.21.
d3433c0ee89c414f41b7d99bcec92359463db5ab Deleted tinymce files and removed from project.
b8b9c9ddc035c321388df4c5450676da029315ee Styles, tweaks, and added codemirror to CKEditor for luxurious HTML source editing.
ed110234bd711b8fe74c74e0d90dfb896a9c5062 typo: fileselection -> filesselection
4c6ccabae5e12b076ea2e10066629612c4a87a6f Remove superseded FreeTextArea.js.
c9ae4d414da947603ad6129fa9e95d8c53e5fc4f Include ckeditor into VS project. Exclude tinymce.
75901028c300cc3bb1177603cccc2cc9fe020ab5 Update FreeTextArea to make it work with CKEditor.
cb3a22044c6b62c000db201620eca45004304bdc Upgrade ckeditor to 4.1 RC. Eliminates the need for customized filebrowser plugin.
aab820c99adfe032212728b37e69d43aaf3bde90 Tweaked ck editor
ba1015ef7e6b45ea195de66261df7d6493d95959 Enable selecting current file in file picker popup
183646cf2ca8c8ffca97ff640e14a0652dba32ad File browser integration spike
991204b33f39b5e9d23c0a1a4d5c0545baa5b769 Updated SQLite to 1.0.84.0
016c4f9a5099cf7d6296e423814324ad79d6424b Merge remote-tracking branch 'bherila/ckeditor' into ckeditor
9442afcfc43abcbe2acaa04cf55603736dfa53d2 Fixed some unit tests
65f69cd6a9f7f387b37f120a4add78567556ac76 Merge branch 'master' of github.com:n2cms/n2cms
6ae2e592ab6ec7313b3cd34fbd13ca8086d5668f Merge pull request #82 from bherila/master
a8c226eaf2168213261dd10c09a885e06c55ec83 Assembly version 2.4.12.20
e5cb064382e053ba6e56666aae61b5f083c6aaaf Renamed tinymce resources to ckeditor.
e0b41ed6be60ffb9c38e3c8b67c5c391567ebe4d Applied patch Opera submit bug in Edit.aspx (discussions/435415)
1e932dd1d934d76f7f6daaa3719d08d151383bb8 Fix for saving boolean properties in medium trust
7982a8c794af0e0cbd191e8a05c8fffc2ebb7095 Fixed problem serializing ascii control characters
c2c0a2d86c7b9d4ea44bbca125668b58343a7690 Removed coded username
4722870a223c291b4a3acd36283a5636b0499f75 NH cache ReadWrite -> NonstrictReadWrite
539589b7b134ce59e70268c2a8e261acd755238d Merge commit '5598d385513fd157b33da9399abbed4797c7c5e4'
317f45306e8c22a5a98e7074c2073e39fe5cbb39 Fix bug in CsvImport and DelimitedDataDisplay
e60a5f2bdbe49aa7ee13c6a460fe32f91f89f8f6 Updated todo with a new idea
5598d385513fd157b33da9399abbed4797c7c5e4 Fixed read-only user was allowed to unpublish page
364deb1f25edc121aecce5134236af0f67f99fad Assembly version 2.4.12.19
a21a88bf7f83c9fbfa03c700c06103f8e74316cd Removed TraceLogger
2e598ce1cc907fec88295222348915d4590fdc6e Removed file from project
e9293152b73598720bef81cc974e60989414dcd5 Move token config to N2/web.config (discussions/434865)
0c9eb117ba483fa7437546cf822c3afc9c05fbf2 More diagnostic info
145897c92145c35b2b5fcf1b76dbfa8388f41934 renamed DelimitedDataUtility to CsvImport
3e0ee852f6515e1b51896deee8ad2565bc07b68c Login form bugfix: Doesn't always redirect after login.
3af44f21a7ee2b95b9092e19839b71ac6e4aef04 DelimitedDataUtility bugfix: Header rows duplicated
0a4e968520ac0c5eeab1fd0ba62e63c7e4e8bfb4 Assembly version 2.4.12.18
08bdc8fb7d8065d41792465615fc3eb25c7a0ef4 Possibly fixed problem saving roles
8f52365c85f616c9a625d3f40b8b9f88a7ef4e98 Merge pull request #81 from bherila/master
63e02dcc77e676e754c5f4901a9f6e0279766429 increment assembly version
0a034611d32c2470a3c62e4d263e5036130b230a fixed bug in LoginForm
57b35670ea685264e2033b5ad634ff33ffe975d1 fixed bug in Delimited Data Display
784718de94dd27b5f13679a45b47812b94bc3334 fixed bug in Slideshow
21e0d9d7880e217c49b15bd6cdb35f6011b23077 Added DelimitedDataDisplay part and Redirect page.
2eab0ed543fb497cbb8a0aec36b71d805903185f Added restricted content regions and redirect options to LoginForm.
2db85d9e927da7699d7585cbe759de4f46a4f810 Added LoginForm part.
7a759eacdfc40d57318a51a3b6d3c5de93e6d564 Normalized whitespace in WebForms PermissionDenied handler and StartPage item.
447044b1c904ce254ceaf4944258a4677fb7eadc Updated project file and assembly version.
38a5e97dd4561dec2d162dab5a6cb46373ef27db Added File Download List part to Dinamico.
a347f594670a6eb1a6aada61cb06cf00ce41ba16 Refactored slideshow recursive directory list into a utility method.
4b9689310138f681b5b61b2593a3f6e99961d863 Added Permission Denied handler to Dinamico.
c7e601e2e33401d0509464d4e38a112357e51d69 Removed overwriting of edit tree link again
580cef3531b97c2d72d23afec2e734cf30087255 Merged origin->master
a58f443114605eb6ded7ae422bb650019ad36e7c Included CssClass which was used by a view
a4f12230209405488215f51a7c980457ef0664b1 Merge remote-tracking branch 'bherila/master' into newcontentparts
93310772c94421ff3739a0030387721b31814af1 Merge branch 'master' into mongodb
d7f6a1dc5bf7677b6e6cbb38fad03baf9564a104 More likely fix for  discussions/400284
1ddaac1b952e044ba8e7f78c72d967a8624c80fb Merge pull request #80 from DejanMilicic/master
aec5956ffd09132ea51425b529a108eb913540d4 Possible fix for discussions/400284
e842299a823909515ee684543356d59934c2ed12 Revert "Possible fix for discussions/400284"
b055b68d2893582a30c8d983e6b572a7444055e0 Merge branch 'master' of https://github.com/n2cms/n2cms
fe14953395839606b41ea984e5dc6075c8a6e619 Fixed some things related to relations
bdf57a25eaaae2b1949fb3e42eac77528584a9ae Removed overwriting of edit tree link again
90b1fa511550cf022913a29e41d9a42cae2f9e63 Some changes to improve backward compatibility
79c8547fbfcc6715783cba26eafd4f283d9c090e Content relations tweaks
042944bd030efb9324d7c06553799fc3b10ebc61 Resolved an issue serializing relations
6b042cc791bf31b33bbec164479e275120eaff4a Typo corrected
f36f65aeb3ad3e7e4b2cfa99dabbc421b947d708 Possible fix for discussions/400284
aeb9b6e4c3b3c765d8ffa7e73895090231b242a1 Merge branch 'master' of git://github.com/n2cms/n2cms
414a0f80941f322d5ad3d1032bb22951be3e54e5 Retrieve preview url from node adapter when saving pages
50c712ea050c807bccce4c61232d963af6614e91 updatd todo
14f452b95236af73248e53f697a0d1ddae7cbf8f Fixed some unit tests
d51d2004825c65487183160addd68f673701c5d7 Removed unused jQuery (and added link to Google hosted copy just in case) from CSharp example.
9b5ff7620632a7943a80da9eb19908e3ee4852bf Fixed service injection into content instances
afda7dd2eeac367351ef7c680751627bec0d3837 updated bootstrap resources to 2.3.0
f1ace6ffb86458e8d13076036f41944cefe22e9a Added support for recursive zones in Dinamico.
60839a844d3264f8907fda6688eee5d4931f5683 increment assembly version
9c3e6052ddfcb81fcc5453f554eed93222d73c63 add Brazilian Portugese translation
4ba3f7243ab37abd1146717dc450fb34a41a0ced Implemented installation manager for mongo db
326995c7fd736f5d18f465cb69a962074481eac3 Normalized whitespace
d4681382ff833ba2f6db176e75966bebd042cc91 Update Bootstrap from 2.0.3 to 2.3.0.
f197e17f18b076b4658be2ddb19e07dddd7ebf4c Added slideshow to a page in mvc template demo site for testing purposes.
ae9b9da06cab871b8d34f7b2da0583f2e8b09952 Normalized tabs in ContentSlider
819805d3ea818b7602e9ce1b55e3929151a2571c updated Dinamico csproj to include new files
719a62ba4d0d77f4c1cec573f15e3e3e37e316cd Updated Galleria from 1.2.7 to 1.2.9
b2199ec920f4a8590b9d8e22e9176e34736168c7 Added Hyperlink content part to Dinamico.
ea5f8fedd7b41dd317f4e0acb31a0675b3d35bb4 Slideshow now working and tested in Dinamico and Mvc (db updated).
551aa8da18a6686b8fbd0a238294661701e5f9a2 Slideshow now working and tested in both Dinamico and Mvc.
418c92a714f26947412af9742fec3a182ee88521 Query tweak
3a9c43bbd436773d94e0ff69905136d7b8f96773 Fixed state not set when importing old-style export files
9934739447a9cb1364f4f2ee989cb69b657b511e Tweaked deserialization to achieve unique objects within a session
0f9f19dc3d00ad03dc68ca5ef044a47435defec0 Reverted a breaking change
081e05d86fc22e00e6e7426df885acd0aaaea436 Fixed delete
a3b9cce7a4ab8df4b36e6b5d72c1c259f14195bb Updated some code to use repository instead of finder
b4b61441cbe42f818897d56e9c2ad20c0a5cf916 Workaround against stack overflow when parsing ContentItem references
43a308f50d4bb6519d94a7ff59c85526923be7f5 Changed finder to repository
d76ac9686541d3f06deefc3bf68fd02208776b4e Changed some finder usages to repository
a324421f04c250bf74bbb667ded3d503f5d96760 Fixed some issues related to overflow when deserializing content items
fd3ec5155ed43e317d1cad9ae3fd43c0ba737d17 Changed ContentDetail.LinkedItem to reference by ContentRelation rather than ContentItem
64b9621d5340703a92a904cfa732fd83526cc0c0 Changed some uses of finder to use repository
2e0010d2d7a2a939eedaea30fb870bacabbccc9f Merge branch 'master' of git://github.com/n2cms/n2cms
05e4c1aac761bbf171770146284259fc2c17510e made the GetLanguageGateway functions virtual to allow overriding
1d6ec4ad8db1a7a30be38cf0730d5f389fa748b2 incremented version
41f55eb988189e5e882f16c1c0cd8dadc4055bfd typo in comment
e2699fe541867533a6e846b4df2b7305c4324a45 fixed bug with date grouping with items are not sorted by publish date
eff21d920184ccfdaf83d7f0d7654b843e1ce39d Bugfixes and improvements to ContentList. Added HTML header and footer.
0b632e2373adf6ab9324c1a125b7c22ef777349f Fixed bug in GDocViewer.
5b44a3af0e5498715ff3236b08eca634648bbfe2 Added EnforcePermissions option to content list.
96edd35cd9a9ad5fb38da373293b0292c8e3e1dc fix whitespace in n2.proj file
b91ba6cb7f40edb238eb08413d65e2ee451b0af9 Refactoring
d3a1e4790bbb327f5b7a0d369939a5a5550f34bb Fixed detail queries for mongo db
bc612971d9c50dd02bfc6778144648bc49c3b63c Fixed a bunch of query scenarios
901bfa08b4bb2bd94304e18212ac67e52713c1a0 Improved mongo support in regards to detail collections, and detail querying
3001d30ee4bd6da80fbc9aa2d924aed160872a6e updated assembly versions
804da423264be6187a699ad6058c22c458887dc9 bug fixes
42efb7cd62363d8198771db8244398f281f1ee34 Added support for Property.In to mongo repository
b437dce51de019a3defbced0dd8766ccb1a336ae Added support for Property.In for repository Find
52d861466f4b99482152212970b76efee167e2df ContentList: fix whitespace
90fff94b2e5c59f75f51886c5f95ae9592e57a58 fix whitespace
3b8e14785785eac8a9d7237be66ff9fd9de91438 typo: datbase -> database
a13702728e04fe92c6425202962189d31d6d9997 Various fixes related to versioning
055bf439741da14d46b81194177310af75993e18 updated ImageAndText to get CSS styles working
d7d99eb2bf57f1f2083e276df16076a1e7d48b14 cleaned up ImageAndText part
b19bde6a09dfdecde04b9f8ca78f48bc81777a5a removed excess CSS styles from ImageAndText
617143a6f77105b88dfbc07612c13c901ca2dca2 updated assembly version
0ec1ef5c7034a0ff8c2e6976c0d60cf0969b40e1 Added some extra sample parts to the default database
ce0961998beb627819a2cb2d767c83a8ba05dae5 Added ImageAndText to project.
89e27802af758f32c5f1411a1ac169422bf661a4 updated ContentList
b33eb811df9c2aef08306ee40a7f47f48bf5bd9f Added ImageAndText, VariableSubstituter for ContentList and Slideshow, and Slideshow
23a5d9cb9113f26c1d51432a22296d8969d5592a Some improvements to versions
c3097db3fd9e888e81db1d28784d1b3ca2f0ad1c Fixed support for references and removing references
386ade96a97a99d95e75085011479d8a9a487573 Merge branch 'master' of github.com:n2cms/n2cms into mongodb
46dd11858898ba4302256c3f199a2da45d9206d2 Sqlite 1.0.84
b60104fd5a05cbf0fd6c63a4f491a67280bea62f Fixed some unit tests
ec03a793f419d3e4fc498fe9e5e2c66829a40e4a Fixed issue translation finder query
d6a0a227d6cbe780152789691f4227baa98ce2ce Some mongo refactorings
7885dbb75a3b57c00edeb38eef8abc86b4fc19f5 Merge pull request #78 from meixger/master
a00f86ccc6a176e63a668a16b952c2db2152b8eb Various changes to support storage in mongo db (first version that works to some extent)
ac9a946083302b4b0198213f101fbe81d5a0af40 updated ImageFloat to fix some undesired behaviors
3c665198b3bef0e9b2a325e6122e5810a454fae3 Some adaptations to actually using mongo db
95561df4ec687443ce34089097d7a99f7dd36a41 Assign Parent and (lazily) provide children
22bf768bf79686d9c963cb6b8cb0d98eb8a0449e Improved mongo db repository
e2089ce289f404bf983f93f9432839e4404bcecf Fixed a few mongo repository scenarios
8c52693911f63eb361efc2d892e8e02cfb844466 made the GetLanguageGateway functions virtual to allow overriding
c51f397809c968fa12eaa3a814d6c575e2be2b73 Serialize detail to mongo
ef1e31016d04a93466b2b913657f7b7eb1335132 Created N2.MongoDB project and moved some code to it
b585ad4c2b7c9fd05830ad355d2be6677a93a374 Fixed merge issue
6d0904cc857aa7dab9d94705d2d493eb1ae2e623 Merged alexjamesbrown/mongodb -> mongodb
c26b016e799f031dd501662367a11b21d0d4f158 Merge remote-tracking branch 'upstream/master'
7d4fbe9f2643fdd9418b7a0dbed416f83bda0342 Added new content parts: ImageFloat, ContentList,RawHtml, GDocViewer.
53ee6dfef08517322eb02ec0994ecf0c23c0537f Assembly version 2.4.11.1
16369ff03683ffa1bb17075e40c1315fa0f027c4 Fixed issue with old export files and imports related to meta attribute
9edd2806bf91a39399ae9d62211c097228107b94 Worked around build problem with locked sqlite dlls
d9c061343cf082f7f98122f62e4f1efc1c8bbe4b Assembly version 2.4.11
8d6928051ee996c52509735e34154e96bf4a4e55 Fixed multiple file upload when using url prefix
56597ad34b5fe7ad72efe1a95036533082f80d3d Fixed enum serialization in detail collections
2b3e722363d3fd44b21b09c64d731d1457913f47 Added query support for enum detail type
1b299f156db62a3e966ef95d72fe8f91186486ca Added (int)enum to enum type and added some tests
eb23d63145f3f56f5a512ec888bf7752f3ab70db Merge branch 'master' of github.com:n2cms/n2cms
a698da00a498244feb5edb9fd043b3ae9863cfb3 Merge pull request #77 from bherila/master
3f93a77a51992a3944e7abaf1137ac4986fd3b64 Prefer id for rendering parts unless it's a version
b0bebe5798fbe39c369e6dfc16387be8e1d51cb1 Removed sqlite dependency from projects
a87a787c9d317426295dba00757dbb2a3104f26c Possibly stabilizig tests with some error recovery code
bb1c8a2b72ed34965d6768811468771068f0c90e Added pages after treshold to [GroupChildren] an refactored it
bdae266060b4a12978ede8ffb3551d9a1bee6da2 Fixed lazy loading of grouped children (part 2)
8f391ce131918b20de1253696089406258ad8a6e Fixed some tests after definition changes
b95f1f97093ec58ccfec820aa426401039ec490a Fixed issue with direct url and site in virtual directory
a68d476e47cd5b226bb1f5141e0eb5fad67c8b38 Fixed lazy loading of grouped children
cae3be4d2c49557d8f481939f053622c75cab3c6 Merge remote-tracking branch 'upstream/master'
988c0ab27dc991a20dc79893294de5eee1a4dba1 Updated the way Enums are saved to make the system more robust. Now saves fully qualified assembly name + Enum type name + String value of the enum and attempts to re-resolve the Enum value at runtime. It is now possible to retrieve the String value of the enum, without crashing, if the Enum type is no longer available for some reason.
ce8d0493f53cd61b77a2b648ce0ff9c98a3d29ad Used grouped children on news container
5c3eb6db280b344abdba72c8b2dce4e69b766681 Added support for grouped children
e8a00f0290920340cc50bb3730d038dacec97e28 Fixed some odd urls when having extension
4f75c3b1b8edb023758868f67650f2eeee30a231 Use sources when saving items changed by behavior invoker
95644d7bccae827e8eda20bead615aa779665eb5 Avoid loading details when loading collection
6b87e527551e525d0d9d6da7a8fbc5f3d5bd0761 Fixed broken unique name check when saving new items
284b84916f75335effa2bce61d41a494a1ec81e0 Lazy ItemList children, support for Select(ing) specific properties on children
fff42a1645487ad33e599ca0b169c1446d5bf067 made a start on creating mongodb repository and unit tests
d0c421cdc3d97d9e48487770153a052f571feb32 Fixed tracked links updating on newly created parts
cb0ec03a33f28a4fbe26bd33c02f698f8073c88f Better way of not breaking images in tiny mce editor
e5f9ee26673e1bfbe7b853bc8c92ce6a5043be93 Improved update file prefix migration to update items without tracking information
466b657ef09cbfed7aac0a18acd9ccef7b2ee402 Updated find without known detail for in-memory tests
5ef71809196223afb33742bbf2bb766c2760f43f Support for repository find without known detail namegs
c6689fe9abae9ce7d693f834799385c6729e870a Include migration in project
8b3fe9dfd3e2dafb9ce800ab7e6939d046040884 Only publish current version
4a0bbd4e9eefe611f0d5d8df222d4fd2a7fce4ac Assembly version 2.4.10
cb852b743dd7f8ceeb6b39676f515932e4df6ae5 Included the actual migration
e14cac3228c00dad6e37b10dc9b513aa3b9e4156 Added migration that updates links to upload folders with url prefix to include prefix
8a4c80cda4dc74728d240e390e41e4890276be71 Fixed link to image size
65a2ad25046de420bd94277cad76f54d41856f36 Made new items proxies to simplify retrieving links from details (this show now work since children are no longer cascaded)
f340f88c7d7b3dd5e7cb0b4cc567293816a823ec Fixed update references when using urlprefix
8856244427c0df75c399f9df2bb9b39e8c191a5e Fixed operations on file/directory
dafea8fd1d537a4047668363b7e442eeab788f55 Fixed parsing of prefixed paths in management UI
8cdd4287c3c2fc4d2ca27ad681e7fe610774d6fb Fixed inserting external url in editor
c8ed1ed5570562d6b275a36c5bcb69cbdadae674 Fixed some unit tests
0e8d911f1f96d2d62a0d50efa127b0d6891d5610 Fixed issue referencing images by application relative path
a94b25f82c52444fc507c98db63328b6774a6052 Fixed issue saving fluently registered properties
c2f918c5227b5ef695113874ff6eed20eec92d7b Fixed problem with previous commit
d5cca01658254dbc3e71d4eb7517f0899666f568 Included sqlite reference in project files to simplify development
fe25645304e74f565d7b5faaccda99873d11c98c Some changes to support medium trust
053410880e1b633f667980abb56208e9236c29f6 Extended skip loading pattern
6296427a9cf103a657e2742d54b97d68f11edc30 Don't validate integrated mode config
e5aaede72ff9769a0ea8fc668addb54d7eb0dd7f Reset client id mode for /N2 folder to prevent problems when changing this on the application level
7f7716074002f1ccc44a83a15fa35b468b067b3f Fixed issue saving roles (discussions/415153)
233d825dc09f35a774b6f2391f16242388a3e5eb Removed some duplicate views
905508c2b342b336cd6f2fcd0e56cb2668cab96b Fixed unit tests
934a23b9dee4db22bffb9a521bea43e2957a3ca8 Updated upgrade_2.4.txt
d9bf04b2e6d208bd205da17f805a97f1809b7676 Assembly version 2.4.9.2
c7c07c5bbf651343056f0ce380f591e38bd9d9bc Fixed error handler when called from outside web request
8e4776f19e8ed61de1f42fe00a56f2e5e669f212 Fixed some project references
2b3acb69e041490645368083579d64d8e97fd7f2 Always perform draft check when using IsLocallyUnique
dce1b4289aa9c2df18a36af443a1a3797a6356e4 Merge pull request #74 from faester/versionfix
44875e09a789c843c4f1bc881d5b02337ee96e85 Merge pull request #75 from christianfredh/master
b70099d580176c9c5cacbee6cece1c72277fddf6 Remove some code redundancies
7b4b0da195f798597a2fb96d4413fadbb84ad751 Correct delete error text
d2175a1f2818ac87c29b9130e8452b9f87c6c925 Fix spelling and remove some code redundancies
d2c49372973a8c1c708b5649e074ea7f7fa4a866 FakeIntegrityManager implements changed IIntegrityManager impl
8976afca3a0574ef065f4686c8879658d314cad0 Assembly version 2.4.9.1
27bd2c3b4c2a19f82dd5f52c50edefd1ac0269d4 Merge pull request #71 from christianfredh/master
437f4de82eede3a65e270a28373c29e5b91db40a Merge pull request #73 from CarlRaymond/exception_filter
62eb980517e2394f6b6724dc23c17d54b482e5aa Should fix error with future publishing + windsor3
9baa28051d52f86462d56f268e42e439265cbd4e Fixed issue with token parsing
2f668d6983ac3f8d9202dcd438b8ae1995eb1d28 Simple fix for saving drafts multiple times for items with editable name
e08bacf957772493fd91e6d064a255c7e13d9099 Added IExceptionFilter and DefaultExceptionFilter to filter exceptions.
2689282905863836f8dd18f7e69b5f3a1e4a70d6 Changed so UpdateEditor isn't called twice
f4052845bcf23d812f0a54d5b19552a612eccee3 Some tweaks to sql error handler
016bba4b8f6d3a8f42bd345daa073fb7fd7ba092 Merge branch 'master' of github.com:n2cms/n2cms
dd782c925ac4b6eb11185ff137feccc8bc21f2b9 Fixed moving/renaming of directories in image link tracking
0c54159225952762b1b2fb06e8820cd0dc4eb757 Merge pull request #72 from bherila/master
0b36fc7f4968658b8693d537b14f1835713923bc Fixed typo Metadatda -> Metadata
9457111f1c7967d985ccc8482948d9acd518ee29 Assembly version 2.4.9
1b3edd9639e65bf322465b36753a30d34bd389cf Added missing files related to routing to part versions
6f8b697d0a6474b95cf17c3a02be849c8c638203 Fixed issue with rss aggregator caching when displaying version
de095527e542c37a22bc192ea2458d3c0d6784d5 Fixed issue routing to part versions
68852782aa52e64ca64d8363ae0858cc3cacba76 made startpage content nicer in sqlite.db for mvc templates
4ddad53b07df223cc5dedb9078cf6b104c6e47d2 made startpage content nicer in sqlite.db for mvc templates
d41bc540efa1a069f6de9de7caa02c28f999f530 Fixed issue where legacy tracked links wouldn't be updated
3910b69f731df489525bd4363b302a4debad3d08 Fixed issue saving free form part in dinamico
3ed8bfea36432666d78d21cf01a292fa22b897c5 Fixed unit test
190c4da55adfbf8f746052ee0406f1adb24dcfe7 Fixed unit test
16ad300f5c83e583158fa397aa94d7d6408d3f72 Fixed url invalidation when moving directory
c003328d6dd79f08463681738d6666cf83fbb5ef Fixed renaming directory
ed881e21a0adafa4ca490ec58dd2832d65da8bb1 Fixed other images being updated
48092dc13b81c79a4520165be9b45099d053add5 Added test for multiple images on same page
0d8ebeebd60105f9971710aaea37ce3f45bd7148 Fixed logging
f60e9ccb1657bf63359806a6dc1dd3b0d43d4308 Added some logging to tracker
e541060104b5183d45017270aa054cb3f6ec7b24 Scroll position fix
c8ded7b3a10c4bc70e4aa761995280b8f1fce88c Merge branch 'update-image-links' of github.com:n2cms/n2cms into update-image-links
3c672616af87d9a52a199c34c73ce86393bc0a73 fixed directory renaming
7a70d0ac3464d4ed56239430031e553ba435803a Possibly better handling of resize of sizes
bbd0771032b3be0ea3cc8069e107e2c9a089c7a2 Allow using mapped file system outside web request, content generator only linking within created pages
1651d66a57dd90c5045a2594a0546075b932f643 Refresh selection when navigating in preview, redirect & refresh after publishing page
4e658a4745548a137f0ab32bb192bb882789d4da Generator tweak
0e6f4fc6911bda216af591f1a8898dab86c37a64 Allow generating image links with content generator
8236627f0211fd1945b9856e1c5cde7ce0fccc1d Merge branch 'master' into update-image-links
068d0a0b0986e4c8463bdc0b3e2e7a9c2a1963e3 Fixed issue with restored items not being un-expired
00d36da23c1bad106a812448581714f0637dbc17 Added migration for tracking images
e2d6065d04e1ab9546ae3c280e9402cb829b8a41 Re-add images...
438cec097bcc1ce47390922c5625011c599fc248 bug fixes, update links to image children, added test
aa6a33cc1328f59ddb650aebc5958d32e03cffb8 bug fixes, update links to image children, added test
f712b1766d9275f32b4dab76328320155b9533aa Update TrackingLinks when moving/renaming images and folders for images
a0e043274bc579b89126e9c55ce8a428a5147c2a Assembly version 2.4.8
6d3b4ea058c78b5e1ab93d72757755d5943dd405 Added support for custom sql exception handling
7ac9b8b66b7dfae931ffb39584516920e25adf25 Changed trash background delete to delete starting from base
d0705aea7c6d507992418beafdf3e29595021bea Page info viewer and tweaks to content generator
76ccea48a7f8a1c79b2486c28efb06174bd0e034 CountDescendants extension method
2be4ec6f0c9fd5d78a7715400b1c85ff3b5a9c7a Improved content generator async ability
986b86de83a30f2fd932c5225028342bba4e298d Fixed lucene reference from test project
a953c7d7a3b4091d0752333d510c687c074adbbe Really the selected item check when deleting
fa381cd13c7ec5d6722edbab9e4ddcce90e307bf Resize frames scriptically
9d22ef7f3253819a8dd180f33a86a8e7fdbc75f6 Generate lucene document on calling thread when saving items, added support for avoiding materializing child collection when saving ([SortChildren(Append)], [SiblingInsertion(Append)]
4f00cef551be4e43c45f71a7d7a7ce60c4c5490e Avoid materializing large child collection to calculate child state
b073a8f844088a02d3044f010907f709806673da Async trash cleanup
c8d69deed19a28b6ab036402d62b8589ef484546 Fixed query for finding referencing items when deleting
eee59f727e38eb9d8f328c96d63316320b6b7071 Null checks for easier troubleshooting
580618dcb3870857c1034b3ce446eae5ad343aab Fixed some styles in the toolbar area, and moved toolbar buttons out of frame hopefully improving a clipping issue
f1354b8bddb7b157eb018c5d0ce61f91532a2d08 More style tweaking
6a79ccf9b55e22058a6e6f561ef89e4535c764f2 Tweaked tree styles
55095fae0433220d7b23436a7372d2406db7ff64 Fixed shouldly reference
35a8556d8af010c48eede7d0b11777ac78968cbc Merge branch 'master' of github.com:n2cms/n2cms
0d7e6adfbb598e0e96ef93b303d1806c650bae8d Added editable theme editing to start page model
3e617487ef1842bde5a4daba203413b2a7e47597 Assembly version 2.4.7
c61bbdc1590aef6dfc9adaa46b045adca70695fa Merge pull request #67 from andy4711/master
3b1aa82e8bd751a694a6ef1e43458d077be9f35a Do not try to remove empty extensions, as it keeps RemoveExtension from actually doing its work
8f29420577dfe82753f13b860fccc3895ec1323f Merge remote-tracking branch 'vkornov/users-paging' into pull-user-paging
b6a2eed9957dfb95b9e547072d72290db69e1966 Merge pull request #70 from dignite/master
a814f9c187fb9d0e2e93fdd926c15f05ef3667b6 Refactored value accessor interface
91996394c755eddbc1afddc7e7e902bcdb97fd15 EditableChildren to use value accessor
0daf1336f8b849a856ad2b7c17f55c2378751170 Modified EditableItem to use value accessor to support name/zone
17417661d980c6127357c42df49390496d0a5eb5 ContentItem ctor now uses Utility.CurrentTime()
75594d44b6ab6827b081dc59a68107ee9e9e9027 Added migration for changes to [EditableItem]
8a8a5c8b303d5bec0ab08f159e74e921fb16dc2b Assembly version 2.4.6.3
f6ed035dc6dad3f04cd8235332cee8e001622ee7 Added more test pages/parts
77ea57bc04495485f29c080022374d71c14c84e4 Limit draft indication to drafts saved later than published page, made unpublished page indication more distinguished
09f10ba8798d79472b0b507d0d3e5448ab370c03 Disallow editing parts derived from recursive or site zones
cc8751e2b401b1504fe79130205a9434fc26ec64 Maintain sort order when deserializing version
00624cb1d21d28e062279c5e8e463e7e4cb0fc2d Fixed issue with links being cleared when reading version
0d4d8c7ee67a5fb3cf01d61861c8e5bb2d11172f Assembly version 2.4.6.2
d97b7327707809d7bf9c7eebd6a2d1ba9567fb07 Disabled link updates when renaming folders/files
2fc9c72a03a86b40fd4d97b64360eb8bf626bf35 Updated tests
bdcbf560a09a0613d5e4f26a08b01156742a860b Switched dependency to sharpziplib
42d5ae6055b0b8a742541eb1d80c9cdfa41a6136 Fixed concurrency issue in url parsing
37ea4bf7b5d75641f4bb7610290558b85ec3bb54 Assembly version 2.4.6.1
030acc38a11675ecff472d192a27123de6fe24e4 Fixes serialized editable generic lists and versioning
0ed16f883ded9703fa61d29ac6a125e862359e9c Fixed issue with assigning string collections
3d5d19ce5eca0801809f976184bb665f28c72fad (very) preliminary release notes
762c6ff35239a53ca801e217e3f3817713c92c1b Assembly version 2.4.6
5cc47ca80d8d9edbb3a50e020e768e1d1ba917ba Configurable show recent versions
b527b439e6daa56c6942125d5ecd809c35d3d61d Added view as tracked activity (this should keep alive AD login
877cccbda2f1db4ec991e708d0674b4b91d4c476 Create parts from edit while editing draft
43fc4139fcc7931fe90b4fc14f7519b5b0bbdc36 Maintain tree tools when scrolling, force refresh when publishing preview
0ff767b047d5edcf88e977e879251cfcdc847ecc Removed unecessary exception
47f9863e367a34c6dfbdf28143434311ce7f7d7d Refresh navigation when publishing from preview
9d6e141d63e9df81d9e0399f73b46629d85419b7 Editable upload renders images in folder
47617136c2e0dd9d976cc679854535654781c2d8 Updated debug utility
959e239540d4fae39b555547561367b0b2a78471 Fixes issue with editable selection not saving changes on parts
123dc6474cf96f2ab90a02df615f0598c810c22e Hide logout when not using forms auth
038b77b356b0a4bf1bacc76055d5f8832784b937 Set parent of version items
d2749deb6b41248fbf005c153f1fafb874005467 Extra check to avoid crippling editable children for pages
c5af2a8ebfd3daf7ac92ed705873778469c48a07 A little padding to reduce clipping on outside column
309672bd7f8d8ea8f1462bd71a27158d79965cd6 No overflow on free text area label
180965249bd50700c512378ee7a0f0122287c05b Fixed problem with update app path migration (discussions/401479)
714b689b715697d7906cabdb3aef68061876a605 Updated assembly version 2.4.5.2
0a54819cf33bc0e2698ba16832730e46a0ddde35 Allow importing parts without a containing page
ab1c5203fd3acf81d105ff654b6c1aff7fad84de Added validation for direct urls
f87aa1839a6c8e823ffc5dd33ae99c86f81a90f7 Fixed issue chaning direct url to page
fc52224ff59d447212869a4382cec54497db73ea Fixed typo
0796d18db0ca7df8abc5c16c698454a9882102df Fixed installation issue
0218fa807ff269eb8f8dcb970d7417fb2430dee7 Removed AllowPartiallyTrustedCallers
65a6862d1eb9c5dbab0144613a3c83ee9016a976 Fixed some build scripts
908840f1a024fb716ef3205c202a20264914570f Updated some references (mvc, nhibernate and switched to nuget references
279fb046a4cd37a50d46372c20b43fbdf2f45b14 Made some builders in N2 implement IHtmlString directly (and removed factory from N2.Razor)
b28506253193000fd62b21c0f2d177249b44794c Removed windsor 2 package, unbundled lucene
5900c926464b378a55d90b0500e59a09c3567276 Define cosntants in release build
7dede7610315063f017eda5d46c2c9891108d70c Configured legacy request validation mode in N2/web.config
706b5c11460e6df1f16b742e55f3c2ae0977aa93 Changed projects to .NET 4
7da0c9738f640f97d530f5ea90dc608452114381 Assembly version 2.4.5
d809f649ea6133d1814f54a93df351f780154cbc Too many packages.config
a7849c9e17919b35dfc3f351d195db61e2b0b646 Added even date to calendar teaser
7ec54decec7fdc48de765b71a6963d4ff73bfea6 Added property sort direction to item finder
2297492d4a3d7128e10549a74a2b13b63c8ff786 Added json to tests
c0e2c79863ed8006a21ca63a6c0d1848329df652 Fixed parts adapter in mvc templates
f4a14fa20e12a66b5a36cafd344d796e4363738d Test parts for editable item and editable children
b9fa5d1206c20da9e3c5a5eefe1091bf02740633 Disabled scheduled actions while debugging to the invoker (configurable)
23d2912ac96548a4e7315f17b8a31f7d8c60eddb Fixed problem cascading changes when using behavior invokers
320b378bd452bfddba2c42dcb88ee09b281547fd Fixed editable children tests
24b1ed1bcdcbe25c1b37a58f229ed330e9030c07 Fixed some more unit tests
1a582bdd761d3d6286a2fa68f98521fb2898828c Fixed copying of deep hierarchies
ceb37e1b8068248eb4fb2d3205bf9671699be380 Fixed some item finder tests
b669384832d7398e57ca1d3a6440e57919e01ae2 Fixed cascade delete of children when publishing draft
73b2b5c410e3d75870e170628b96a6827af154c6 Fixed some item repository tests
564cb282242cc996567b1dba0889dae95a6ae49d Fixed language structure test
8ea5f4431db8991f812eb1ff1ba883a52d358acf Fixed intercepted children property test
3af42ce7ec77d32b4651c6ea550c4d51dfe68003 Fixed validate definition test
2e3d44ed92fc58d6459f084ffe6bf99d0a846c19 Fixed editable children tests
92936f4b70c6289920b88229fc559cc62ce9937c Fixed versioning tests
f6eacdd92609effa4ba23daef5958c1ee68e77c6 Changed definition attribute to avoid overwritting unintended values (sort order)
8ef23f9eb448499e2f657f1374eaf13f4fb8208f Fixed editable children
a3064f7cc13b7e51d37bffe99908635fdcead39a Fixed editable item again
0a6873f78130f2fa82e66c34c5f911e06b42fc5b Some fixes to [EditableChildren]
d37547f56522eb346a6a3f0a09399e3e6fcec3f6 Fixed saving [EditableItem] with the new publishing workflow
173b6682b2c91e0a0618cd9686cf8018a1636100 Improved debugger display texts
c45303639c4fb8341dad09f751e43b405a4f5388 Amend
34f3c25187e9cbc3dba9b18d0da7d1f42323e5a0 Tweaked logging
d67d287a0ecb2906b7726d4c2020a140e47646f0 Fixed some invalid models
0b9645818c83b4befa0ed080bbf3d752a3ef391d Make proxy [EditableChildren] retrieve children
c696d75cf8af1e2de930cbd746c2c46632c05b53 Make proxy with [EditableItem] retrieve from children
c1a14bd9c9cbae90edd62eeddca6bbd482169f2c Fixed creator node, and other situations where drag&drop state wasn't compared correctly
ade4122728421957c1a3a05ce7b94f57775180a6 Fixed indexing of encoded characters
b87e296215050029485978c036eb4b480b9b12f3 Changed Redirect to parent page to render parent page in test part
6b678fa2528201d9ac88cc1c2f3597094404aaaf More project tweaks
9c378b7c60e5a0035eb5dc28a9650d544e56780a Include lucene dlls in management package to prevent issues with prerelease packages
1705c97db3fbce35b330245caa40634e5b54ab17 Removed examples prepare msbuild
e8cd3422d8f9c82d2503e350430e4d74ab80224c Removed prepare examples bat file
b1c00ec5ce1aa64104916d233566935d524d9f37 Updated examples to latest version
8f7697e246525e0a6c4bce8dc7556f527520c0db Overwrite db file with master's version
04561d503b36d362d246b0fa94dcd9467e4c488c Changed port again
4c79011b400e39017b761b00d1b80e77e3e26f35 Changed poprt on iis express
777a2e4d78bd95e2b8c8a2cc3d91b3c19cdce217 Merged master -> vs2012
46d89e6de5ae37a279af7607dd073c91c2710a67 Added support for synchronous actions
02d742977945978cbb2edd637442d2e31c65d9d7 Fixed publish button for parts using [Versionable] to allow direct publishing
23dd5b7321a80d1a95a15f027e868d8b2a3fe909 Made some test parts non-versionable
f1475265141a5278d3926161d955e52f96b815ee Updated version 2.4.4 (beta-4)
2fe9487caff8aa37238aac1cfca430c88776a6d6 Switched to IIS express in project files
dce36dfcdf5fb35e9ad744c900d16eae2f3755e8 Made link button more prominent on url picker
cde355f10d90ca2c7c970174a4ca53442ecaa158 Updated profile provider
9e785d8cbff8230bf0f62382e6fb4723cf8a8d7b Updated connection string examples
7079bb0bdd2aefd562c92152ce9808bf0fe139da Fixed so synchronous indexing doesn't close the nh session
a2304d99115b83bf4572ec238b377a75aa8dff26 Added option to begin transaction on first session open
cc5e2351486618a5cffff9f95e5cda5096907864 Changed logging around nh transaction wrapper
1e96f52aae8bf96c921944057230a9f8d06d05bf NonstrictReadWrite -> ReadWrite
801f5181f4143108eff79392cacfdace96a94a12 Added helepr class for logging with log4net
4f8f992928c6505cbe66d64927709a863164bbc3 Allow saving parts without a parent page
a7806a1bc884fc3463a5d75b575dd12f1edcb386 Simplify logging with log4net (via nh)
a1428b143a518119e0dbbb41c09be3b8dccc23bb Reduce work when throwing items in trash by only resetting state
06deb04fbf24c819188b205b8e9e429f7e687d52 Updated config xsd
05002c0006d67fa864ae3d72ca1a3c8bb2cfdff7 Moved url caching to caching decorator
65213bea44b9c17d0e07b01af5f872534ee6e2d0 Unpublished tweak
18c367c2b067840b4fe266945b45e549005c8bb2 Tweaked styles
77c3bdbe4de0e1924bc0619f47b177df52e01820 Added configurable option to make to unauthorized requests (unpublished/unauthorized) return 404 rather than 401
3257bed47ff83a4033ba7ea65e6129c41fcb116c Tweaked icons in navigation
57771f425f3f4a1dce21882a69d02528112a71bf Remove unpublish for auto-generated items
b488ff6d32b0bda444e4b5b3be274ed9807952d5 Tweaked view preference selector
e9a903cc4109fc9fa9a6009ee9dd6a2b35ec1ae9 Fixed invalid url when organizing
111468235dc6b95299d85f925b1718092a76945e Enable direct url validation
1fe267914dd05d2024d94f9632e052f778af728d Fixed issue with finder and versions (transient object exception)
c4e6d40b1218765f2ce8071dd187a756318f47f2 Cached direct url
5f429bbe494083898f5bf24b4215dd0cf3017307 Some fixes and implemented url source on news container
caa5a3ab84f3fb3908dbf4fd0aa12d070d4351e4 IUrlSource which allows overriding urls at a deeper level
d2279b91d33af71b5c0144482f8ab9e8e927f35d Tweaked test
f9e72c0dfa340261b165fca0bb49d2de8c1f8d8b Added some tests for children association
2ded909c861b3002f0173d476d2d2428b3c35a41 Fixed some problems when upgradring from 2.2
c7460d4ea40e87d99fa1713a4f36b9ef24b2dbd2 Restored items to correct path
6fdf0688fb0eb69f8e85ae23142b0cd232077166 Removed items with wrong path
76d667a878dc6a65c87dda11f7e468bcd0e1e353 Move upload to last
7c2667ba8f725863b89eac79f48c872e855e4afc Upgraded database
27ef218598995dc9ea18f761b717f6894c570cca Obsoleted google analytics part for root page
ed339878b513d5c5731d77d7d8e3445aee4ec03b Update status after upgrade
c82f324fbea35f1d1c3b89e4143289e5467407c4 Nested parts are published with page
6e7a84e9e94828cf37503020cff383b8cfbcb016 Fixed issue with null autoimplemented properties and versioning
6c00142282d4c383fe34ed3caf0f9fd723120298 Fixed issue versioning auto-implemented properties
6d7324558f3f9894858fa26f74b51cc692a65d2b Removed redundant bat file
d41c6d42388253f19bfb0d8dbb8e7c1b93acabb5 Fixed problem with migrating versions
775198a357273dbf2c37d1f398902ee96769534a New version 2.4-beta3
730c34190236633e2386a559876cc8657702da46 Tweaked icons
3b708bb62dc121c5ef9bb9c9886aae55017296f3 Fixed problem with deleted parts causing transient objec exception
566366644fa66991663344dc8a2f39cda456dfff Maintain part state when publishing changes
95482e496b1e1c013f3cdaf7be7dd17b4b8dfd72 Fixed issue with nested containers
abd6c81063e9a40c96b11d15cd4cdf75ea94e65d Fixed some control panel state issues
a1986cbd05eea8ddc99b99823082b6a938d8ec64 Fixed some merge issues
2c271627c6786ae407ca5ec7d68935c82ae26298 Merged master -> publishingfeatures
83d73c2878d6452857d72905c06d0dc0401b635e Merge branch 'publishingfeatures' of github.com:n2cms/n2cms into publishingfeatures
678920ec4f23e446e423754014bd8d1a66035556 Support for migrating versions to version table
68f9b376398611ecd8e43d9ae00b33175233c0e1 Fixed some merge issues
7abc1fdebddc0fdea309dc3d22734dbe3f3f7abe Merged 2.3.4.1 -> master
d9eb46a2c913cf8b4264b147b2cd579247e8aa98 Publish items saved through editable children
10d7ba8188f52079c2a99cf57c15801b70792314 Reduced serialization when no existing draft
2e0701988abeb0c0a1360d7b1468225fc39269f5 implemented paging on Users screen
c861ccbbea5faab12e992813a0606e912ea491c2 Increased min height margin when opening parts dialog, to solve jumping action bar in Chrome.
f86c47736f50224a83cfddaf26028258c7e4a890 Fixed ancestrail trail updating which fixes problems deleting references to deep hierarchies
91fa93764ffe6d21c682f612b67c292d76e7ed08 Fixed control panel unit tests
799cf1d5081e15c4c597fe9bbad1eb37bb74575d Fixed CachingUrlParser unit test
30d1f698a30c9eef9fd2126c62da457a49930078 Future publish
e45ad2dc6006ce00e4ab07966bfe207b7435925b Consolidated logic for being published
c99612a9fbb7cc412e263701fd30194d7493077c Tried to reproduce strange error
a522a104ac0b508eb624be7c7cab59759c35b964 Fixed issue with self-referencing versions causing publish error
ed4d3e37d71ddd6b5169e5c3c2974382b9113b42 fixed importing old exports
4b2379c7dc17d07e2a2b9515c915613214f0904a Removed debugging texts from edit page
10610440a0e07ecbb7a3c77c95206ba135624c1a Show drafts list and fixed adding parts to root page
d258be3de6a4d1f253fa177e2e054c9951c62ded Merged 2.3.4 -> master
c7c06a907cffcdb4cbf60b772ca0e41df54deac0 new batch file
8b30dc81c2333fcd9a819bcaa56828d7d5883315 Fixed a unit test
83b99394435a9f8529893eac4220f517aab268d2 ied some unit tests
5b0bb7dd9575fa014fb2744a1ea0fae577ff213a Force save of enclosing item of links to deleted pages to update cache
39b849e68aeff39a91156d243e832aef99b362bb Removed wasiniitialized check from remove detail since it may cause problems with cached collections
716642ce893e6790925a19a54d3db4e6105b4782 Added support for unpublishing via button
d61fd1a5ed1712f566f4feed827b5acf43f9a2c7 Fixed issue moving files
cdfb84051d8818ee850b29e0b5d5ef70cb4377ed Assembly version 2.3.4
a037f178f140249443c3e27448d5ec091f15d25d Initial drafts in activity
29bedb0f6c239504de5880c721104dca45a8ae0e Changes for webforms
07215f5e4eed8b16e4fef72460aba631d45ae93e Draft icon in tree
96d81f1107a784f1d9aa3ea725823d7112298c98 Added expires and item count to versions
e3642e2123bbe4637613bfba724d19afedc8c789 Copy parts
b5352a95aab72ae68f02604af7a5c12f9ae05e38 Redirect to draft when moving arts
5ad1c9d3ee66fe23621b7f345ce54660146a9e5d Move on draft
7ef6884c8d522ff772d3aaa443d066a14f923595 refactoring: vi -> versionIndex
b45174cae1ba0f80a120ed041f0fb2dab3cdd113 Better versoin handling support
bf3cf96b4b7cf6c7f9d2d23991c38b3a3598bfab Support for adding part to part container
223adc469f8c0222df6c6cc9dadfa2ced0c7bd67 Clean versions when removing item
a5745e8f3e9b08688e34a841527c52ce184955df Same behavior when publishing from button and version list
6bb939316cdb2ffb959469a11000e03c67002f3a Restore parts that have been deleted
f6a3cf0864b5c158ac39e32408f4c1290e1f807a Insert parts on draft
b10603ac1b66446a7b5a3932d8a5d6f11406f8de Edit single creates draft which is previewed before being published
eb90f00abc776f1d4e02de15894ee6487bf81b2a Fixed some unit tests
6ca6b59853ab4949d8432528c415b899f85a0fba Added missing projects to n2.everything.snl
8021719ac3fad833c077e701a0a758767f6b3bae Fixed merge issues, referenced lucene from packages
8f5ea63dd7f5797ce4d8f4d74531e947c9844c19 Moved VersionManager to N2.Edit.Versioning
b90f55bdb6c66d06f6960b53fc872ee3d5f0dad6 Updated wiki code
50d03fe01ec1c16da26eecf2df8f0823b7458b27 Merged 2.3.3.3 -> master
7ee8db0ab9d3d2871f1599db04fe2e4b3f4fe3dc More changes to async indexing, shorten session lifetime
1deb8dfd0b647bf33e1b9911871804c5cc30ace2 Added some logging to publishing action
5392056b8fe0adc5f6b3f0a38842e5b1d8598f56 Handling of index clesed exception
cab34493d074820c10edde1d0923191330f92ba4 Updated version to 2.3.3.3
2d47b476737367f548a2d23dda0164ca78596812 Some commits around direct repository usage (discussions/397524)
ff17da083bbcf41a058bea262bb723109a65a19d Changed so async indexer uses a single thread at a time
ff212bfd65aafd23b6c17a5d48ec433210b41056 Back to drag after add part
b07ff2c24e58bc57a247d6e8565e20cc2a83b893 Made default values work with versionoing and export/import
628248b33da2358ffe16392d54425bc2d162a9f6 Maintain more UI states when using buttons
ff4c98c7b97bafd14081f8508dbd274e79d1c753 Fixed delete part
dc9913a0a4e7855b1d9ce59c9ff6e7ead3a91f74 Versioning of persistable properties
e371a2b7af9c9d5c17c36ab0c2e6825f62b6c6b0 Fade out sliding curtain while dragging
d0aca030ec53805250eee1029e6830b40045d880 Made it possible to update existing parts
d874578284421537e080279efe8ec835c9aabbac Parts are published with page and children cache is updated
39666a4f486300ebcf1a29da2797a1a3a582de95 Update published parts when publishing draft
0b15938c352e6c888686b468d35c00ee854f9cbe Add and modify multiple parts on draft while keeping the same draft
4b7961a4352b4c6b76aeddfed0c6a31fac010af8 Added support for modifying existing parts on draft and publishing, consolidated control panel buttons somewhat
7ec55dc84c3f27efc6197004399e3e8acc9e386e Store and view versioned parts
73eb9a02e28bf805fe6e5753af99276d286cb4ec Serialize parts, show parts in drafts
e2b610675053c2347dca362b0d7ac381c74a534f Fixes related to saving versions, linking versions
9a9332a08faefb38b1b31d1ec5bcdf61cf1f8305 Fixed organize link from management
0945b714dc19c7802c2e4fa525ecf417af161173 Moved some buttons
e53237d6ffa2f0d4828ceb22e88e61480b4f26a4 Edit versioned item
bdd8bc96f2464fff393c6db4359bdf9a5d1ef709 Keep selection when switching between draft/published
35127acbf028919537f4d5bf35e3e4de6084ff28 Improved distinct version index support
fc10fad6487e554771643b05835b966d90660d46 Added support for deleting and creating versions from the UI
3f66b7308eb33f51ee308c6d36009484ae196865 Merge branch 'cherrycs' of github.com:n2cms/n2cms into cherrycs
8192b2da4d7724acc2b883927244256db37bc62e Implemented external storage of versions
a55a06a13a60fb8300c39497c5c45c5ec8cce844 Marked selected toolbar option.
c33f5135bc7239fd93652fcbacb6c2b37a1f5379 Merge branch 'cherrycs' of github.com:n2cms/n2cms into cherrycs
33c2dd3c0462854ce0e032754e1423c181cabca8 Moved Published/Draft setting to Pages tab.
5ff349f8b0ae3be0339d5c488015c38b135c9290 Merge branch 'cherrycs' of github.com:n2cms/n2cms into cherrycs
88566c8b035c646338d9ab0b752f7a5e6b234340 Remove detail from enclosing item to update cache
467b13ce91575f85f58e0bad8ad7c3548ab130aa Fixes after trash changes
c3aeee22ae6d10351c51eef9f3335823bd10dcf4 Merged master -> cherrycs
e1ae97d28ade31f0659ed9ecbe92af2937585341 Merge commit '929e2e279349322a273a64a0b1498669c8d23f82'
d15c4d484025852e510bb36b69190f22af9e1e04 Assembly version 2.3.3.2
929e2e279349322a273a64a0b1498669c8d23f82 Fixed a weird scenario deleting details from an unitialized enclosing item
834fdea7b02441c3eda8a800ad5c64e7d0d84eeb Merged 2.3.3.1 -> master
d9bc9be0ffe0da828c631c4f9beda843c0836ab4 Added unit test
3a3a5f83167356bd851890ae39e2417b015cdd9b Assembly version 2.3.3.1
5af392e0c9aa71f3b89dbd0305f5ec8051f01dcb Changed so deleting items also removes relations contained in multi-values
dbb05f41bb1d1904741278fb9f54a51522dda541 Amend last commit
8478b3f521a30ca684677a4997301d639081f556 Amend last commit
57752dcc1b64073f597e24a10632afa822b8c0cd Added debug ifdef for tests code in mvc templates
27e3d959dbb520f096596720f86831244b41c294 Assembly version 2.3.3
fe6c4d1c73e6b0ccaa8322978bab0f45aab76509 Merged hotfix_2.3.3 -> master
5e525b8471bea5acae4ffeb5f1858a9b30c01f28 Link tracker doesn't track ISystemNode
d6b2dd83b64280cdb652edb660d67b52e4c19107 Reduced impact when saving membership users, cached results for GetUser and GetRolesForUser
a896e75734b8d1448d192efa6f7ab0336118be9a Changed so children arn't batched by default, added batching to details, collections and roles
3c6c29a5aa1c6919d91d299be908dcea82dee852 Allow content reference to be broken
69907158426b024dee2dec88ac71f2bb3155067a New way of storing versions
36205aaa5e4c1554d4ed4d1f7c4abb19eb633d38 Cherry-picked improved test content creation
5f1d060cd54ba26cd84054c6f1ac97ac389eedf0 Refactored mvc test parts
22f6becdfffbcd646aeb772a2041ec8f7a7131fd Improved content generator
34ca31b95b90fffb7bea418da15ec89b58cd1bc9 Merged origin->cherrycs
1859557ed851a8694e3c84f6a8b63ccb62c71ba7 Refactored to view=draft/view=published
bb1b3560ef3980993124e412fe887cec1fe16397 Merge branch 'cherrycs' of github.com:n2cms/n2cms into cherrycs
23eda601fcd527cec31cfb692927ae186ab7d3b9 Inject ContentVersionRepository, fixed tests
c8c44d8b9a1f7734c09fae08a82d105fa4b08c97 Added configurable viewmode with query string support
4ca363b2f50c00d096856a8ac2b5d31eea33249c Merge branch 'cherrycs' of github.com:n2cms/n2cms into cherrycs
0c2f5a25c22f9a03779cdf08572be606108fa406 Moved toolbox to tree view.
76409ccf6211455c265c908834d9f6d0426ce6fe Updated db file with new schema
b5951346ee351e2c9cd90a4a73da6e6f6e83dec7 Restored web.config
1cd05c4346d4b6c926cc390f4b9dab323c6326a3 Merged origin -> cherrycs
3f93312517ec03be662cbcdfac0ebb32317b86b1 Improved content version handling
8340f5eb6549de555bfd3dc1ad58650a8f97da73 This is not the push you're looking for
86d1a51d32f0f95a024ce30b650662c32cccb14d ControlPanelState should use Flags
9091745af56eec635e7f8310e9d2579a9da027cd Tweaks
6bb892aab209d803e0653a7947b39939ecf26783 Changes to support tracking links only for published items
fe8268d5f80e498543a5b8c3cb6b52c91e7b379f Fixed some merge issues
d45292f54534397d6c37d3317a0baf020ee2c9f9 Merged contentsources -> publishingfeatures
2171ce5dadcdb61cc2aa575df5886acfeeb83509 Initial version structure
338efe56ffe98899108d30e690f77d8dd2c11520 Added token finder unit test
3c3608a924c72b8c73dd953199b13d2ae918bc7b Remember token complete setting second time
67457df45521bba71c53177b215b900f9acd83f2 Referenced castle from new location
b9339876e5b2326529bd6813992c28c7d52b8720 Merge commit '578ac93'
1689c1bdfc1e7cc941ef6a85138a8f00232db5c5 Made sync child collection state consider enabled flag
a8c385d2a5fc76614b6ce458cc876dfee036d364 2.3.2.1
578ac9387aca95da76084f7a1f273ffe824a612b Fixed culture from config in scheduled actions
217d09d78f74e76e6b2b703207ea69eac6e99026 Allow disabling of token dropdown, fixed issue with edit single
3f20e23c66866065ab3cc478cad7bf94a4560fb8 Allow disabling built-in tokens
51abcc7d54e69260c1885253231efc54f090e715 Added token finder, fixed json parsing and implemented rudimentary keyboard support for token dropdown
ced1a6d6fa39865c7f5758e65c072390409cb7e4 Merge branch 'master' of github.com:n2cms/n2cms
9203b8797e5c2563a6cb860c6b4774389349efd4 Disable tokencomplete plugin by default
c90730bb13bc44c0158812ca5e76855d3d25b1f4 Disable tokencomplete plugin by default
e98caf53e102b22ab12e3fc6cce4889714c0bc5d Merge branch 'master' of github.com:n2cms/n2cms
3096d43ab749fc87f410e4866cbe49cc2439904b Updated concurrentcy test
7eaec04f809cc28e2fee7d2d7ba0e791c434ac64 Merge branch 'master' of github.com:n2cms/n2cms
744897960572bffb47fcc66de0d9cecb4350dd76 Merged hotfix_2.3.2 -> master
821df2d62f4dbcf393b65db3c6bb822ed18337e9 Updated versions to 2.3.2
bd863ec8cdab07856d679516be8b52870fbf305c Fixed obsolete warning
9cd31fe3a58e5c4a61f07d1c48a77240d42b44b0 Cherry-picked e9a9a4aa1bcb0f4a4d8800aa99f2e23e3e7af688 'Changed editable tags to simplify testing'
df44c8017be96c07a11471df9b815c5db518cde7 Updated lucene to 3.0.2
21141e88b20c6497d088e964400db7b3997128e9 Cherry-picked 663c314915807721812a4afcb8a5a0ff517e4580 'Displayed configured UI culture in diagnostic screen'
4dca677fd3a7e1de34de7ae561e604663a518de2 Implemented config-bound culture when executing scheduled actions
efe120fb3c6fc311d2cd0e242cb491bc09bce506 updated .gitignore
f11f961499759483fb99dc6ccfe4974e511e8cc9 Tweaked dynamic proxy to allow intercepting only necessary fields
d4fa29414198ed6dd5a844e30fc21a3ee9f66414 Link tracker doesn't track ISystemNode
f779aff159bbd5f8c9135f0e846f9b68aa6755dd Reduced impact when saving membership users, cached results for GetUser and GetRolesForUser
7481070f2a4ac0019c6de8135292f99b96a9193e Fixed misspelling
be02f1fee4e1a54c6eb46cb01f1b0fb0fccbecd2 Updated projects and examples to VS2012
01e13bfe095b0bdcba1eb9cbf6fad6ea69fe6e7e Updated project files to vs2012
0eed47b39a45c2bf465d51e3b42f04445bd0fdb6 ReadOnly extended EditableTextAttribute
066b5c298a69389aa444b3435528166fcda391a3 Merge branch 'master' of github.com:n2cms/n2cms
4c0609465b8d916bc4e88628d98d31d751c2789e Merge pull request #66 from nazjunaid/patch-1
0dec25ccae855d927df8850260dc5f795bad9bf3 Wrong content being returned if using NonRewritableAttribute
0ceb3e16f40cbcbdbda5837b0ef3ac095aa9b99e Removed double packages config
6a9cb94861037a7762064110aef9880f09ae868c Merge pull request #63 from vkornov/auto-publish-checks
ed8fc517e6b2544ff2da756878ae9281776432fb Merge branch 'master' of github.com:n2cms/n2cms
8adc4438f7fe738b519c1db5158c224071f58bbb Added missing "this" to GetControlPanelState
b6380e86fbdd7b04b34429da22c91b7b3ad39c58 Merge pull request #65 from GrimaceOfDespair/Local_2.3
4c98d906815bfc4fd45b7534906ac099ef3619b9 Consolidated zip udage to use SharpZipLib
ff73216f8302c5f2c486247c6ebcc2bffdfb77e8 Fixed unit test
a55113597490b24a02dcb654212d0d17289684dd Removed unused references
5f45c2df2065611fc2fe176d093529f2598f84ce Merge remote branch 'nazjunaid/updatelucene' into updatelucene
91c0aa0bcd46cfa2cffb4567fb71dc4b7946b9bf Remove duplicate mention of FormCheckbox
3ef8638cbf43a03f3e468007183afadd953ab6e5 "recenet versions" --> "recent versions"
865d88bc658c9bf849d53145049720e4613b170d Improved version of comparision
a26a45111f16781a637634bf3e18879ef4741f72 Merge branch 'master' into updatelucene
5af6d026778538eb151311227dbae809e9ce8e53 Removed fallback to detail collection when resolving item[name]
a6c839787b4424841b1df85de2900334e3f671df Refactored language gateway to avoid a database query
b77be7541cc3ad4463e36924ff2505a681638e5f Extended rendering system to support both webforms and mvc
6862652a81f6e9f109a5370cb8356e8b17d91ada Fixed failng test
6b76c0775fd8ceadd4facea01a1ba104c7d63cd1 Refactored dinamico startpage to fluent registration
a10a37e0d4d9dfd6817bd868fcebb6230b2cd70b Order registerers by inheritance depth
e4c004a1dd2a8c6dd5a00776d6e8eda9a3c9ba86 Moved some template registration extension methods to N2.Definitions.Runtime namespace
8e71267cb80331656f685fe4e2e75d490ea4236f Changed FreeForm registration to use fluent for regsitering controller
4a52b2d77228b76f4fcf57c9b89d22aa1ed24bc1 Fixed regsitration.ControlledBy so it works with controller mapper
d71665015536d7a030dac005c9420331f0df8db0 Model tweak
8467333abade4f108a977adf43a5dccf31213c70 Fixed form tokens for other web configs
2d077178e02213984b192afb441b03d4934b1daa Added freeform to dinamico
55f2422b2ed2e9a535d6139d2df4dbad5424693e Tweaks to fluent registration
5bceb7eb18ab15e609d717697b057f5e4443bb83 Upgraded N2 to use Lucene 3.0.3-RC2
ec5232d6cd5ba094195b0b57fb26791e1df3cb7f Added support for selecting detail values only with item finder
a705ab9e191f5a530c59dc52b319769c97937119 OutputCache html helper
14dce50d9f40fe75e9ae1519f9484d69c75811bb Updated nuget version
47ba6b420078b8c0e121b464c6e52440022def4d Changed to auto-properties
663c314915807721812a4afcb8a5a0ff517e4580 Displayed configured UI culture in diagnostic screen
e75f185abc4ae2349197886847953d359cafcb21 Implemented config-bound culture when executing scheduled actions
08815cd8a630bd54464d08e7a6567ec81fcd22c7 Fixed model to avoid problem with proxied start page
445e2a8f1d7eb4375a195e6102696cfd3d53c3da Tweaked theme
80e8b97bc5bcb529ea1260d19c3736c159707dd8 Added default theme used when no theme is selected
cc48ab33e582e14b72b9e732ff379fbdcaa73344 Specified attribute namespace to prent issues when including other references
d4f09471873bfab1ca91f43f3b6ca90dc942e5c4 Added and enabled TinyIoC as default IoC container, added Windsor2 and Windsor2 as separate projects
a1c719b2d897fc73340735f7e037fec97e5a534f Changed logger to support medium trust under .NET 3.5
2df56cf5f5d6cf4a7fac9ae7cd6dbbc212bbe611 Updated config schema
e0c70cf904f74a185a62a52d16ae51404d65e2c2 Included castle dependencies as package
d094d653836dc5802e59ea4b47433cc0352a0ae2 Fixed issue with resources when using N2.zip
ddbdcc9baf50e9837852a5284d6b64b5b9e42777 Changed nuget version to 2.4-beta
f181674b4be2c94be33443199dae833df4787aeb Specified attribute namespace to avoid conflicts with more recent versions
5dffc42940b53ce4a318c75bb3b7120f2a2cef95 .gitignore
4c4eab13694685dfb9a4595d58240e02f37fb1ce Created nuget packages for windsor 2 and 3
ec035fbbae30de22099372c838cdaa36cb075d28 Fixed windsor3 ioc support, reduced dependencies in nuget package
b5aaebd0a0545ffed64627337a994502d8fd294c Merge branch 'master' of github.com:n2cms/n2cms
1831c4dec2514da4140d410255ef66a04d4a64a0 fixing pesky null ref error in auto-publishing
3ee060d38214893456193ddea457788f4da8b297 arg null checks on public methods
b1e6158c8973f05db2a625afa112310937136315 Made container type more easily configurable
3afae5f20a089b3bf3ab99eaaa9593997d134bbc Reduced dependencies from N2 dll; embedded TinyIoC (now default IoC container), extracted Windsor to separate assembly, embedded DynamicProxy2
2bac45c447aac4406e7cd6371a5b9a32a2e2a132 Merge branch 'master' of github.com:n2cms/n2cms
59b6031ed985b72e4c8a3a1ab30fa3c8fecfce75 Fixed unit tests
40954030a690e06e226cb6636a60669b2872e6cc Merged master -> contentsources
a767b81f14703f5508c2ee6de6f20b8c47e96bec lol
9fcd85f4eae88b5eb5ee8314114f429b80e1a4f8 lol
0dc86afd842ab521e72f49befc18628fc00a723a Fixed transactions with content sources
3dfdbb29840b03226982c9b370dc1fde8ca340b9 Merged master -> contentsources
ffb4ab18a03d6a4b5bb0c727704752b7c977a33f Fixed some unit tests
818fa6aadadec89b680ae39b7b741dcc4e4c46b0 Merge branch 'master' into contentsources
4bdbf2fb971677f966c92fff64ade2270f63769d Merge branch 'master' into contentsources
b9b0d0fb8a3abac9cded2c6b6b6474cc443aafa9 Merge branch 'master' into contentsources
6db6e580204a48d944d4b64cbf8c388980d71d9b Added repository find detail support, changed so scheduled publishing removes version to be published after publishing
11a2b46dcbaa43d60849f75ed5d8055b20195114 Merged master -> contentsources
f1412abf488ff58c152043f38f53eebce94d4229 Restored location for imminent merge
08179677fdfa45980b9a6ea82c5fa0091c2f920b Merge branch 'master' into contentsources
24a1498e366903110ac2acb1686ed3a45994c6f4 Merged master -> contentsources
92108ba1c323251ff16d2595b20d881b858af8e4 Refactored ContentPersister to use sources
b1c5988c2c58e003870237b195e873fd31121691 Refactored code to remove need of item finder
5803819ab3c1dada4bbb8248156f3ec556246c8c Refactored persister to use icontentrepository
a7cc28a1b155bd4ff8cad70a1164bd664c7cdba2 added ContentItemRepository.RemoveReferencesToRecursive
0de87fee8108689535d5cc37f370cfe5926afc1c Added src/Framework/Tests/App.config
1397d281125e17b6b37202e835a77824a6769936 Refactored code to support content persister dependency on content sources
6fa5e455f8140c4f00bf037632b49fc44755d6b5 Moved ContentPersister to N2.Persistence namespace
bbf9816341fcc4178f19ed1accafa94c620bab75 More diagnostic info
f44347d5e6096935dbd44b9c6c3844d95cfae2ce Added tests for active content
