export const Validater = {
  isEmail: (email) => {
    if (email == null) {
      return false;
    }
    const re =
      /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email.trim()).toLowerCase());
  },
  isPhone: (phone) => {
    if (phone == null) {
      return false;
    }
    const re =
      /^0(32|33|34|35|36|37|38|39|52|56|58|59|70|76|77|78|79|81|82|83|84|85|86|87|88|89|90|91|92|93|94|96|97|98|99)+([0-9]{7})$/g;
    return re.test(String(phone.trim()));
  },
  isNumber: (number) => {
    if (number == null) {
      return false;
    }
    const re = /^\d+$/;
    return re.test(String(number.trim()));
  },
  isCitizenId: (citizenId) => {
    if (citizenId == null) {
      return false;
    }
    const re = /^(\d{9}|\d{12})$/;
    return re.test(String(citizenId.trim()));
  },
  isTaxCode: (taxCode) => {
    if (taxCode == null) {
      return false;
    }
    const re = /^\d{10}$/;
    return re.test(String(taxCode.trim()));
  },
  isContaintSpecialCharacterForName: (str) => {
    console.log(str);
    if (str == null) {
      return false;
    }
    const re = /[~`!#$%\^&*+=\-\[\]\\;,./{}|\\":<>\?]/;
    return re.test(String(str.trim()));
  },
  isContaintSpecialCharacter: (str) => {
    console.log(str);
    if (str == null) {
      return false;
    }
    const re = /[~`!#$%\^&*+=\-\[\]\\';,./{}|\\":<>\?]/;
    return re.test(String(str.trim()));
  },
};
