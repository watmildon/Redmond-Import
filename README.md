# Redmond-Import
Process King County GIS address data for OSM import
# Sonoma County Building/Parcel/Address Import

Based on https://github.com/zyphlar/sonoma-import/

Please see TODO:Create OSM wikipage for the official project page.

Join [OSMUS Slack](https://slack.openstreetmap.us/) #sonoma-import channel for questions/comments.

## Project Status

File generation is complete: https://github.com/watmildon/Redmond-Import/blob/master/Redmond-Import/Addresses_in_Redmond___address_point_fixed.geojson

Approval for import pending.

## Field mapping
-ADDR_NUM -> addr:housenumber
-FULLNAME -> addr:street via getformattedstreetname(FULL_ST_NA)
-Unit -> addr:unit
-POSTALCTYNAME -> addr:city
-ZIP5-PLUS4 -> addr:postcode

TODO: Add state info?

## Import and validation

TODO: set up tasking for this import

Please double check which user you are logged into JOSM with. Ensure you are logged in under a dedicated import account so that it's easy for OSM volunteers to separate your normal edits from mass edits: a name like "jsmith_import" is good and obvious. You can do this by going to Edit > Preferences > OSM Server > Test Access Token.

If you haven’t contributed to a building import project before, please choose a task in one of the more sparsely populated parts of the county.

- Open JOSM and enable remote control. (Edit > Preferences > Remote Control)
- Ensure you have the Conflation plugin installed (Edit > Preferences > Plugins)
- In the tasking manager, click "Start Editor" to load the overall task area in JOSM. (You can use iD to validate a task, but *do not* use it to complete a task. Ask a project coordinator if you need help with JOSM.)
- Click the Tasking Manager link under "Specific Task Information" to load the import task’s data, which contains address data from King County for Redmond.
- Enable your aerial imagery of choice in JOSM, and offset it ("Imagery"→"New offset") to match the Redmond data. Bing has good local imagery.
- For address nodes inside of existing buildings, use the [Conflation plugin](https://wiki.openstreetmap.org/wiki/JOSM/Plugins/Conflation#Usage).
- For address nodes with no building, move them to the main working layer for eventual upload.
- If an existing feature has address imformation, coalesce and correct the tagging on the existing item.
- Run the JOSM validator. Ignore any warnings that don't involve buildings or addresses. Focus on the following warnings and errors that may be related to the buildings you have added:
  - Duplicate housenumber
  - Housenumber without street
- Upload the data with the following information:
  - Comment: `Imported addresses for Redmond WA #RedmondWaAddressImport`
  - Source: `King County`
- Mark the task as complete.

### Data

- 26,073 addresses in the King County data file
- ~5000 objects in Redmond have address data already
